using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineGameConditions : Node
{
	private PlayerControllerVariables _playerControllerVariables;

	[Export] private bool _mineExcavationTimeTriggered;
	
	public override void _Ready()
	{
		InitializeDiInstaller();
		SubscribeToActions();
	}

	private void InitializeDiInstaller()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnOneHourPassed += PlayerFaintFirstWarning;
		MineActions.OnOneHourPassed += PlayerFaintSecondWarning;
		MineActions.OnOneHourPassed += PlayerFaintOnMidnight;
		MineActions.OnOneDayPassed += MiningDurationEndsMoveBackToMuseum;
	}

	private async void PlayerFaintOnMidnight(int hours)
	{
		GD.Print("An Hour Passed: "+hours);
		if(hours != 24) return;
		
		_playerControllerVariables.PlayerHealth = 0;
		GD.Print("Player fainted");
		await ReferenceStorage.Instance.MinePopUp.ShowPopUp("Player fainted");
		ReferenceStorage.Instance.SceneLoader.LoadMuseumScene();
	}
	
	private async void PlayerFaintFirstWarning(int hours)
	{
		GD.Print("An Hour Passed: "+hours);
		if(hours != 20) return;
		GD.Print("Player faint first warning");
		await ReferenceStorage.Instance.MinePopUp.ShowPopUp("It's getting late");
		MineActions.OnPlayerReachFirstWarning?.Invoke();
	}
	
	private async void MiningDurationEndsMoveBackToMuseum(int days)
	{
		GD.Print("Days Passed: "+days);
		if(days != 6) return;
		GD.Print("Mining duration over, player moving back to museum");
		_playerControllerVariables.CanMove = false;
		ReferenceStorage.Instance.AutoAnimationController.SetProcess(false);
		ReferenceStorage.Instance.AutoAnimationController.SetPhysicsProcess(false);
		await ReferenceStorage.Instance.MinePopUp.ShowPopUp("5 days of mining expedition are over, going back to museum", 5);
		await ReferenceStorage.Instance.SceneTransition.FadeIn();
		ReferenceStorage.Instance.SceneLoader.LoadMuseumScene();
	}
	
	private async void PlayerFaintSecondWarning(int hours)
	{
		GD.Print("An Hour Passed: "+hours);
		if(hours != 23) return;
		GD.Print("Player faint first warning");
		await ReferenceStorage.Instance.MinePopUp.ShowPopUp("Feeling drowsy");
	}

	public override void _ExitTree()
	{
		MineActions.OnOneHourPassed -= PlayerFaintFirstWarning;
		MineActions.OnOneHourPassed -= PlayerFaintSecondWarning;
		MineActions.OnOneHourPassed -= PlayerFaintOnMidnight;
		MineActions.OnOneDayPassed -= MiningDurationEndsMoveBackToMuseum;
	}
}