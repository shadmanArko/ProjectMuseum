using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Tutorial_System;

public partial class MineTutorial : Node
{
	private PlayerControllerVariables _playerControllerVariables;
	[Export] private TutorialSystem _tutorialSystem;
	
	private bool _moveLeftAndRightCompleted;
	private bool _digOrdinaryCellCompleted;
	private bool _digArtifactCellCompleted;
	private bool _switchToBrushCompleted;
	private bool _brushArtifactCellCompleted;
	private bool _startMiniGameCompleted;
	private bool _idleToClimbCompleted;
	private bool _climbToIdleCompleted;
	
	public override void _Ready()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		SubscribeToActions();
		SetProcess(false);
	}

	private void SubscribeToActions()
	{
		MuseumActions.OnPlayerPerformedTutorialRequiringAction += CheckPlayerTutorialsCompleted;
	}

	public async void PlayMineTutorials()
	{
		await Task.Delay(1000);
		MuseumActions.PlayTutorial?.Invoke(6);
		SetProcess(true);
	}

	public override void _Process(double delta)
	{
		if (_tutorialSystem.GetCurrentTutorialSceneEntry() == "Tut6a")
		{
			_playerControllerVariables.CanMoveLeftAndRight = true;
			_playerControllerVariables.CanAttack = false;
			_playerControllerVariables.CanBrush = false;
			_playerControllerVariables.CanDig = false;
			_playerControllerVariables.CanToggleClimb = false;
		}
		else if(_tutorialSystem.GetCurrentTutorialSceneEntry() is "Tut6b" or "Tut6c")
		{
			_playerControllerVariables.CanMoveLeftAndRight = false;
			_playerControllerVariables.CanAttack = false;
			_playerControllerVariables.CanBrush = false;
			_playerControllerVariables.CanDig = true;
			_playerControllerVariables.CanToggleClimb = false;
		}
		else if(_tutorialSystem.GetCurrentTutorialSceneEntry() == "Tut6d")
		{
			_playerControllerVariables.CanMoveLeftAndRight = false;
			_playerControllerVariables.CanAttack = false;
			_playerControllerVariables.CanBrush = false;
			_playerControllerVariables.CanDig = false;
			_playerControllerVariables.CanToggleClimb = false;
		}
		else if(_tutorialSystem.GetCurrentTutorialSceneEntry() == "Tut6e")
		{
			_playerControllerVariables.CanMoveLeftAndRight = false;
			_playerControllerVariables.CanAttack = false;
			_playerControllerVariables.CanBrush = true;
			_playerControllerVariables.CanDig = false;
			_playerControllerVariables.CanToggleClimb = false;
		}
		else if(_tutorialSystem.GetCurrentTutorialSceneEntry() == "Tut6f")
		{
			_playerControllerVariables.CanMoveLeftAndRight = false;
			_playerControllerVariables.CanAttack = false;
			_playerControllerVariables.CanBrush = false;
			_playerControllerVariables.CanDig = false;
			_playerControllerVariables.CanToggleClimb = false;
		}
		else if(_tutorialSystem.GetCurrentTutorialSceneEntry() == "Tut6g")
		{
			_playerControllerVariables.CanMoveLeftAndRight = false;
			_playerControllerVariables.CanAttack = false;
			_playerControllerVariables.CanBrush = false;
			_playerControllerVariables.CanDig = false;
			_playerControllerVariables.CanToggleClimb = true;
		}
		else
		{
			_playerControllerVariables.CanMoveLeftAndRight = true;
			_playerControllerVariables.CanAttack = true;
			_playerControllerVariables.CanBrush = true;
			_playerControllerVariables.CanDig = true;
			_playerControllerVariables.CanToggleClimb = true;
		}
	}

	private void CheckMoveLeftAndRightComplete()
	{
		
	}

	private void CheckPlayerTutorialsCompleted(string str)
	{
		
	}
}