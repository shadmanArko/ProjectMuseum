using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Tutorial_System;

public partial class MineTutorial : Node
{
	
	private PlayerControllerVariables _playerControllerVariables;
	[Export] private TutorialSystem _tutorialSystem;

	private HttpRequest _getPlayerInfoHttpRequest;
	// private HttpRequest _addTutorialArtifactToMine;

	private bool _isMineTutorialDonePlaying;
	
	public override void _Ready()
	{
		CreateHttpRequest();
		// GetPlayerInfo();
		LoadPlayerInfo();
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		SubscribeToActions();
		SetProcess(false);
		
		MoveLeftAndRightTutorial();
	}

	private void CreateHttpRequest()
	{
		// _getPlayerInfoHttpRequest = new HttpRequest();
		// AddChild(_getPlayerInfoHttpRequest);
		// _getPlayerInfoHttpRequest.RequestCompleted += OnGetPlayerInfoHttpRequestCompleted;
		
		// _addTutorialArtifactToMine = new HttpRequest();
		// AddChild(_addTutorialArtifactToMine);
	}

	#region Get Player Info

	private void GetPlayerInfo()
	{
		// _getPlayerInfoHttpRequest.Request(ApiAddress.PlayerApiPath + "GetPlayerInfo");
		if(_playerControllerVariables.PlayerInfo.CompletedStoryScene == 10)
			MuseumActions.PlayStoryScene?.Invoke(11);
	}

	private void OnGetPlayerInfoHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		
		if(_playerControllerVariables.PlayerInfo.CompletedStoryScene == 10)
			MuseumActions.PlayStoryScene?.Invoke(11);
	}

	#endregion

	private void LoadPlayerInfo()
	{
		var saveData = SaveLoadService.Load();
		_playerControllerVariables.PlayerInfo = saveData.PlayerInfo;
	}

	private void SubscribeToActions()
	{
		MuseumActions.TutorialSceneEntryEnded += BasicMineTutorialEnded;
		MineActions.OnPlayerReachFirstWarning += GetPlayerInfo;
		
		MuseumActions.TutorialSceneEntryEnded += DigOrdinaryAndArtifactCellTutorial;
		MuseumActions.TutorialSceneEntryEnded += SelectPickaxeTutorial;
		MuseumActions.TutorialSceneEntryEnded += PlayMiniGameTutorial;
		MuseumActions.TutorialSceneEntryEnded += ToggleClimbTutorial;
		MuseumActions.TutorialSceneEntryEnded += BasicMineTutorialEnded;
	}

	public bool IsMineTutorialPlaying()
	{
		if (_isMineTutorialDonePlaying) return false;
		
		// GD.Print($"Tutorial No: "+PlayerInfo.CompletedTutorialScene);
		if (_playerControllerVariables.PlayerInfo == null) return false;
		return _playerControllerVariables.PlayerInfo.CompletedTutorialScene == 5;
	}
	
	public async Task PlayMineTutorials()
	{
		MuseumActions.PlayTutorial?.Invoke(6);
		await Task.Delay(1500);
		SetProcess(true);
	}
    
	private void MoveLeftAndRightTutorial()
	{
		_playerControllerVariables.CanMoveLeftAndRight = true;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.CanToggleClimb = false;
	}
	
	private void SelectPickaxeTutorial(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo is not "Tut6a") return;
		GD.Print("after return statement in ended tutorial number "+entryNo);
		_playerControllerVariables.CanMoveLeftAndRight = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.CanToggleClimb = false;
	}
	
	private void DigOrdinaryAndArtifactCellTutorial(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if (entryNo is not "Tut6b") return;
		_playerControllerVariables.CanMoveLeftAndRight = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanDig = true;
		_playerControllerVariables.CanToggleClimb = false;
	}
	
	private void PlayMiniGameTutorial(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo != "Tut6d") return;
		_playerControllerVariables.CanMoveLeftAndRight = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.CanToggleClimb = false;
	}
	
	private void ToggleClimbTutorial(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo != "Tut6e") return;
		
		_playerControllerVariables.CanMoveLeftAndRight = false;
		_playerControllerVariables.CanAttack = false;
		_playerControllerVariables.CanDig = false;
		_playerControllerVariables.CanToggleClimb = true;
	}

	private void BasicMineTutorialEnded(string entryNo)
	{
		GD.Print($"Ended scene Entry No: {entryNo}");
		if(entryNo != "Tut6f") return;
		SetProcess(false);
		_playerControllerVariables.CanMoveLeftAndRight = true;
		_playerControllerVariables.CanAttack = true;
		_playerControllerVariables.CanDig = true;
		_playerControllerVariables.CanToggleClimb = true;
		_isMineTutorialDonePlaying = true;
	}

	public override void _ExitTree()
	{
		MuseumActions.TutorialSceneEntryEnded -= BasicMineTutorialEnded;
		MineActions.OnPlayerReachFirstWarning -= GetPlayerInfo;
	}
}