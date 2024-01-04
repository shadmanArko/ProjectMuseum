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
		await ReferenceStorage.Instance.MinePopUp.ShowPopUp("The sun is getting really low");
	}
	
	private async void PlayerFaintSecondWarning(int hours)
	{
		GD.Print("An Hour Passed: "+hours);
		if(hours != 23) return;
		GD.Print("Player faint first warning");
		await ReferenceStorage.Instance.MinePopUp.ShowPopUp("You don't have much time left");
	}

	public override void _ExitTree()
	{
		MineActions.OnOneHourPassed -= PlayerFaintOnMidnight;
	}
}