using System.Text;
using System.Text.Json;
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
	
	[Export] private string _alternateMiniGameScenePath;

	private Vector2I _artifactCellPos;
	public override void _Ready()
	{
		CreateHttpRequests();
		InitializeDiReference();
		SubscribeToActions();
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
		_artifactCellPos = cellPos;
		var scene =
		    ResourceLoader.Load<PackedScene>(_alternateMiniGameScenePath).Instantiate() as
		        AlternateTapMiniGame;
		if (scene is null)
		{
		    GD.PrintErr("COULD NOT instantiate Alternate tap mini game scene. FATAL ERROR");
		    return;
		}
		
		AddChild(scene);
	}
	
	private void MiniGameWon()
	{
		GD.Print("Successfully Extracted Artifact");
		var discoveredArtifactVisualizer = ReferenceStorage.Instance.DiscoveredArtifactVisualizer;
		var cell = _mineGenerationVariables.GetCell(_artifactCellPos);
		GD.Print($"ARTIFACT CELL IS : ({cell.PositionX},{cell.PositionY}), {cell.ArtifactId}");
		//discoveredArtifactVisualizer.ShowDiscoveredArtifactVisualizerUi(cell.ArtifactId);
		SendArtifactToInventory(cell.ArtifactId);
		MineActions.OnArtifactCellBroken?.Invoke(_artifactCellPos);
		_playerControllerVariables.CanMove = true;
	}
	
	private void MiniGameLost()
	{
		GD.Print("Failed to Extract Artifact");
		MineActions.OnArtifactCellBroken?.Invoke(_artifactCellPos);
		//TODO: Show A Popup that says artifact lost
		_playerControllerVariables.CanMove = true;
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
		MineActions.OnArtifactSuccessfullyRetrieved?.Invoke(artifact);
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