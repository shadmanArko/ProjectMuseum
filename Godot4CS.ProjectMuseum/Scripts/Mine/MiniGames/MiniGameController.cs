using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;

public partial class MiniGameController : Node2D
{
	private HttpRequest _sendArtifactToInventoryHttpRequest;

	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;
	
	[Export] private string[] _miniGameScenePaths;
	private Random _random;

	private Vector2I _artifactCellPos;
	public override void _Ready()
	{
		CreateHttpRequests();
		InitializeDiReference();
		SubscribeToActions();
		_random = new Random();
	}

	private void InitializeDiReference()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnMiniGameLoad += LoadAlternateTapMiniGame;
		MineActions.OnMiniGameWon += MiniGameWon;
		MineActions.OnMiniGameLost += MiniGameLost;
	}
	
	private void CreateHttpRequests()
	{
		_sendArtifactToInventoryHttpRequest = new HttpRequest();
		AddChild(_sendArtifactToInventoryHttpRequest);
		_sendArtifactToInventoryHttpRequest.RequestCompleted += OnSendArtifactToInventoryHttpRequestCompleted;
	}

	private void LoadAlternateTapMiniGame(Vector2I cellPos)
	{
		CeasePlayerMovementDuringMiniGame();
		var randomMiniGamePath = _miniGameScenePaths[_random.Next(0, _miniGameScenePaths.Length)];
		_artifactCellPos = cellPos;
		var scene =
		    ResourceLoader.Load<PackedScene>(randomMiniGamePath).Instantiate();
		if (scene is null)
		{
		    GD.PrintErr("COULD NOT instantiate Alternate tap mini game scene. FATAL ERROR");
		    return;
		}
		
		AddChild(scene);
	}
	
	private async void MiniGameWon()
	{
		var animationController = _playerControllerVariables.Player.animationController;
		animationController.Play("celebrate");
		await Task.Delay(Mathf.CeilToInt(animationController.CurrentAnimationLength * 1000));
		var cell = _mineGenerationVariables.GetCell(_artifactCellPos);
		SendArtifactToInventory(cell.ArtifactId);
		MineActions.OnArtifactCellBroken?.Invoke(_artifactCellPos);
		ContinuePlayerMovementAfterMiniGame();
	}
	
	private void MiniGameLost()
	{
		_playerControllerVariables.Player.animationController.PlayAnimation("idle");
		MineActions.OnArtifactCellBroken?.Invoke(_artifactCellPos);
		ContinuePlayerMovementAfterMiniGame();
	}
	
	private void CeasePlayerMovementDuringMiniGame()
	{
		_playerControllerVariables.CanMove = false;
		_playerControllerVariables.CanToggleClimb = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.Player.animationController.PlayAnimation("brush");
	}

	private void ContinuePlayerMovementAfterMiniGame()
	{
		_playerControllerVariables.CanMove = true;
		_playerControllerVariables.CanToggleClimb = true;
		_playerControllerVariables.CanAttack = true;
		_playerControllerVariables.CanDig = true;
	}
	
	#region Send Artifact To Inventory

	private void SendArtifactToInventory(string artifactId)
	{
		var url = $"{ApiAddress.MineApiPath}SendArtifactToInventory/{artifactId}";
		_sendArtifactToInventoryHttpRequest.Request(url);

		GD.Print($"HTTP REQUEST FOR SENDING ARTIFACT TO INVENTORY (1)");
	}

	private void OnSendArtifactToInventoryHttpRequestCompleted(long result, long responseCode, string[] headers,
		byte[] body)
	{
		GD.Print("Successfully sent artifact to inventory");
		var jsonStr = Encoding.UTF8.GetString(body);
		var artifact = JsonSerializer.Deserialize<Artifact>(jsonStr);
		GD.Print($"artifact pos: {artifact.PositionX}, {artifact.PositionY}");
		MineActions.OnArtifactSuccessfullyRetrieved?.Invoke(artifact);
		MineActions.OnInventoryUpdate?.Invoke();
		MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("MiniGamesWon");
	}

	#endregion

	public override void _ExitTree()
	{
		MineActions.OnMiniGameLoad -= LoadAlternateTapMiniGame;
		MineActions.OnMiniGameWon -= MiniGameWon;
		MineActions.OnMiniGameLost -= MiniGameLost;
	}
}