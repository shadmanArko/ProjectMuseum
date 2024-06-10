using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;

public partial class MiniGameController : Node2D
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;
	private RawArtifactDTO _rawArtifactDto;
	
	[Export] private string[] _miniGameScenePaths;
	private Random _random;

	private Vector2I _artifactCellPos;
	public override void _Ready()
	{
		InitializeDiReference();
		SubscribeToActions();
		_random = new Random();
	}

	private void InitializeDiReference()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
		_rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnMiniGameLoad += LoadMiniGame;
		MineActions.OnMiniGameWon += MiniGameWon;
		MineActions.OnMiniGameLost += MiniGameLost;
	}
	

	private void LoadMiniGame(Vector2I cellPos)
	{
		CeasePlayerMovementDuringMiniGame();
		var mineTutorial = ReferenceStorage.Instance.MineTutorial;
		var randomMiniGame = mineTutorial.IsMineTutorialPlaying() ? 0 : _random.Next(0, _miniGameScenePaths.Length);
		var randomMiniGamePath = _miniGameScenePaths[randomMiniGame];
		_artifactCellPos = cellPos;
		var scene = ResourceLoader.Load<PackedScene>(randomMiniGamePath).Instantiate();
		if (scene is null)
		{
		    GD.PrintErr("COULD NOT instantiate Alternate tap mini game scene. FATAL ERROR");
		    return;
		}
		
		AddChild(scene);
	}
	
	private async void MiniGameWon()
	{
		var animationController = _playerControllerVariables.Player.AnimationController;
		animationController.Play("celebrate");
		await Task.Delay(Mathf.CeilToInt(animationController.CurrentAnimationLength * 1000));
		var cell = _mineGenerationVariables.GetCell(_artifactCellPos);
		RemoveArtifactFromMineAndInstantiateInventoryItem(cell.ArtifactId);
		MineActions.OnArtifactCellBroken?.Invoke(_artifactCellPos);
		ContinuePlayerMovementAfterMiniGame();
		MineActions.OnMiniGameEnded?.Invoke();
	}
	
	private void MiniGameLost()
	{
		var animationToPlay = _playerControllerVariables.State == MotionState.Hanging ? "climb_idle" : "idle";
		_playerControllerVariables.Player.AnimationController.PlayAnimation(animationToPlay);
		MineActions.OnArtifactCellBroken?.Invoke(_artifactCellPos);
		ContinuePlayerMovementAfterMiniGame();
		MineActions.OnMiniGameEnded?.Invoke();
	}
	
	private void CeasePlayerMovementDuringMiniGame()
	{
		_playerControllerVariables.CanMove = false;
		_playerControllerVariables.CanToggleClimb = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.Player.AnimationController.Play("brush");
	}

	private void ContinuePlayerMovementAfterMiniGame()
	{
		_playerControllerVariables.CanMove = true;
		_playerControllerVariables.CanToggleClimb = true;
		_playerControllerVariables.CanAttack = true;
		_playerControllerVariables.CanDig = true;
	}
	
	#region Send Artifact To Inventory

	private void RemoveArtifactFromMineAndInstantiateInventoryItem(string artifactId)
	{
		var artifact = _rawArtifactDto.Artifacts.FirstOrDefault(temp => temp.Id == artifactId);
		if (artifact == null)
		{
			GD.PrintErr($"ERROR: Artifact could not be found. Id: {artifactId}");
			return;
		}
		MineActions.OnArtifactSuccessfullyRetrieved?.Invoke(artifact);
		MineActions.OnInventoryUpdate?.Invoke();
		MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("MiniGamesWon");
		_rawArtifactDto.Artifacts.Remove(artifact);
	}

	#endregion

	public override void _ExitTree()
	{
		MineActions.OnMiniGameLoad -= LoadMiniGame;
		MineActions.OnMiniGameWon -= MiniGameWon;
		MineActions.OnMiniGameLost -= MiniGameLost;
	}
}