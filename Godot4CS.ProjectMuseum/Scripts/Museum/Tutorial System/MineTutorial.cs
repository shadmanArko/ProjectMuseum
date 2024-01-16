using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Tutorial_System;

public partial class MineTutorial : Node
{
	public PlayerInfo PlayerInfo;
	private PlayerControllerVariables _playerControllerVariables;
	[Export] private TutorialSystem _tutorialSystem;

	private HttpRequest _getPlayerInfoHttpRequest;
	private HttpRequest _addTutorialArtifactToMine;
	
	private bool _moveLeftAndRightCompleted;
	private bool _digOrdinaryCellCompleted;
	public bool _digArtifactCellCompleted;
	private bool _switchToBrushCompleted;
	private bool _brushArtifactCellCompleted;
	private bool _startMiniGameCompleted;
	private bool _idleToClimbCompleted;
	private bool _climbToIdleCompleted;
	
	public override void _Ready()
	{
		CreateHttpRequest();
		GetPlayerInfo();
		
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		SubscribeToActions();
		SetProcess(false);
		// _digArtifactCellCompleted = false;
		MoveLeftAndRightTutorial();
	}

	private void CreateHttpRequest()
	{
		_getPlayerInfoHttpRequest = new HttpRequest();
		AddChild(_getPlayerInfoHttpRequest);
		_getPlayerInfoHttpRequest.RequestCompleted += OnGetPlayerInfoHttpRequestCompleted;
		
		_addTutorialArtifactToMine = new HttpRequest();
		AddChild(_addTutorialArtifactToMine);
	}

	#region Get Player Info

	private void GetPlayerInfo()
	{
		_getPlayerInfoHttpRequest.Request(ApiAddress.PlayerApiPath + "GetPlayerInfo");
	}

	private void OnGetPlayerInfoHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		PlayerInfo = JsonSerializer.Deserialize<PlayerInfo>(jsonStr);
		
		if (PlayerInfo.CompletedTutorialScene < 6)
			AddTutorialArtifactToMine();
		
		if(PlayerInfo.CompletedStoryScene == 10)
			MuseumActions.PlayStoryScene?.Invoke(11);
	}

	#endregion

	#region Add Tutorial Artifact To Mine

	private void AddTutorialArtifactToMine()
	{
		_addTutorialArtifactToMine.Request(ApiAddress.MineApiPath + "AddTutorialArtifactToMine");
	}

	#endregion

	private void SubscribeToActions()
	{
		MuseumActions.TutorialSceneEntryEnded += BasicMineTutorialEnded;
		// MuseumActions.TutorialSceneEntryEnded += OnPlayerReachedArtifactBrushTutorial;
		MineActions.OnPlayerReachFirstWarning += GetPlayerInfo;
		
		MuseumActions.TutorialSceneEntryEnded += DigOrdinaryAndArtifactCellTutorial;
		MuseumActions.TutorialSceneEntryEnded += SwitchToBrushTutorial;
		MuseumActions.TutorialSceneEntryEnded += BrushArtifactCellTutorial;
		MuseumActions.TutorialSceneEntryEnded += PlayMiniGameTutorial;
		MuseumActions.TutorialSceneEntryEnded += ToggleClimbTutorial;
		// MuseumActions.TutorialSceneEntryEnded += EndMineTutorial;
		MuseumActions.TutorialSceneEntryEnded += BasicMineTutorialEnded;
	}
    
	public async Task<bool> PlayMineTutorials()
	{
		GD.Print($"Tutorial No: "+PlayerInfo.CompletedTutorialScene);
		if(PlayerInfo.CompletedTutorialScene != 5) return false;
		MuseumActions.PlayTutorial?.Invoke(6);
		await Task.Delay(1500);
		SetProcess(true);
		return true;
	}
    
	private void MoveLeftAndRightTutorial()
	{
		_playerControllerVariables.CanMoveLeftAndRight = true;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanBrush = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.CanToggleClimb = false;
	}
	
	private void DigOrdinaryAndArtifactCellTutorial(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo is not "Tut6a") return;
		_playerControllerVariables.CanMoveLeftAndRight = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanBrush = false;
		_playerControllerVariables.CanDig = true;
		_playerControllerVariables.CanToggleClimb = false;
	}
	
	private void SwitchToBrushTutorial(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo is not "Tut6c") return;
		_playerControllerVariables.CanMoveLeftAndRight = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanBrush = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.CanToggleClimb = false;
	}
	
	private void BrushArtifactCellTutorial(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo != "Tut6d") return;
		
		_playerControllerVariables.CanMoveLeftAndRight = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanBrush = true;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.CanToggleClimb = false;
	}
	
	private void PlayMiniGameTutorial(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo != "Tut6e") return;
		
		_playerControllerVariables.CanMoveLeftAndRight = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanBrush = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.CanToggleClimb = false;
	}
	
	private void ToggleClimbTutorial(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo != "Tut6f") return;
		
		_playerControllerVariables.CanMoveLeftAndRight = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanBrush = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.CanToggleClimb = true;
	}
	
	private void EndMineTutorial(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo != "Tut6g") return;
		_playerControllerVariables.CanMoveLeftAndRight = true;
		_playerControllerVariables.CanAttack = true;
		_playerControllerVariables.CanBrush = true;
		_playerControllerVariables.CanDig = true;
		_playerControllerVariables.CanToggleClimb = true;
	}

	private void BasicMineTutorialEnded(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo != "Tut6g") return;
		SetProcess(false);
		_playerControllerVariables.CanMoveLeftAndRight = true;
		_playerControllerVariables.CanAttack = true;
		_playerControllerVariables.CanBrush = true;
		_playerControllerVariables.CanDig = true;
		_playerControllerVariables.CanToggleClimb = true;
	}

	public string GetCurrentTutorial()
	{
		return _tutorialSystem!.GetCurrentTutorialSceneEntry();
	}

	public override void _ExitTree()
	{
		MuseumActions.TutorialSceneEntryEnded -= BasicMineTutorialEnded;
		// MuseumActions.TutorialSceneEntryEnded -= OnPlayerReachedArtifactBrushTutorial;
		MineActions.OnPlayerReachFirstWarning -= GetPlayerInfo;
	}
}