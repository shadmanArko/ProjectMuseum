using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.DTOs;

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
		MineActions.OnMiniGameLoad += PauseDuringMiniGame;
		MineActions.OnMiniGameWon += MiniGameWon;
		MineActions.OnMiniGameLost += MiniGameLost;
		MineActions.OnMiniGameEnded += UnpauseAfterMiniGame;
		MineActions.OnMiniGameEnded += ContinuePlayerMovementAfterMiniGame;
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

	private void PauseDuringMiniGame(Vector2I vector2I)
	{
		MineActions.OnGamePaused?.Invoke();
	}

	private void UnpauseAfterMiniGame()
	{
		MineActions.OnGameUnpaused?.Invoke();
	}

	private async void MiniGameWon()
	{
		var animationController = _playerControllerVariables.Player.AnimationController;
		animationController.Play("celebrate");
		await Task.Delay(Mathf.CeilToInt(animationController.CurrentAnimationLength * 1000));
		var cell = _mineGenerationVariables.GetCell(_artifactCellPos);
		RemoveArtifactFromMineAndInstantiateInventoryItem(cell.ArtifactId);
		MineActions.OnArtifactCellBroken?.Invoke(_artifactCellPos);
		// ContinuePlayerMovementAfterMiniGame();
		
	}
	
	private void MiniGameLost()
	{
		var animationToPlay = _playerControllerVariables.State == MotionState.Hanging ? "climb_idle" : "idle";
		_playerControllerVariables.Player.AnimationController.Play(animationToPlay);
		MineActions.OnArtifactCellBroken?.Invoke(_artifactCellPos);
		ContinuePlayerMovementAfterMiniGame();
		MineActions.OnMiniGameEnded?.Invoke();
	}
	
	private void CeasePlayerMovementDuringMiniGame()
	{
		_playerControllerVariables.CanMove = false;
		_playerControllerVariables.CanMoveLeftAndRight = false;
		_playerControllerVariables.CanToggleClimb = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.IsBrushing = true;
		_playerControllerVariables.Player.AnimationController.Play("brush");
	}

	public void ContinuePlayerMovementAfterMiniGame()
	{
		_playerControllerVariables.CanMove = true;
		_playerControllerVariables.CanMoveLeftAndRight = true;
		_playerControllerVariables.CanToggleClimb = true;
		_playerControllerVariables.CanAttack = true;
		_playerControllerVariables.CanDig = true;
		_playerControllerVariables.IsBrushing = false;
	}
	
	#region Send Artifact To Inventory

	private void RemoveArtifactFromMineAndInstantiateInventoryItem(string artifactId)
	{
		GD.Print("Showing artifacts from DTO");
		GD.Print($"Artifact to remove from DTO: {artifactId}");
		
		foreach (var artifact1 in _rawArtifactDto.Artifacts)
		{
			GD.Print($"printing artifact before removing from mine: {artifact1.Id}");
		}
		
		var artifact = _rawArtifactDto.Artifacts.FirstOrDefault(temp => temp.Id == artifactId);
		if (artifact == null)
		{
			GD.PrintErr($"ERROR: Artifact could not be found. Id: {artifactId}");
			return;
		}
		
		GD.Print($"artifact: {artifact.RawArtifactId}");
		MineActions.OnArtifactSuccessfullyRetrieved?.Invoke(artifact);
		MineActions.OnInventoryUpdate?.Invoke();
		_rawArtifactDto.Artifacts.Remove(artifact);
	}

	#endregion

	public override void _ExitTree()
	{
		MineActions.OnMiniGameLoad -= LoadMiniGame;
		MineActions.OnMiniGameWon -= MiniGameWon;
		MineActions.OnMiniGameLost -= MiniGameLost;
		MineActions.OnMiniGameLoad += PauseDuringMiniGame;
		MineActions.OnMiniGameEnded += UnpauseAfterMiniGame;
	}
}