using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineGameConditions : Node
{
	private PlayerControllerVariables _playerControllerVariables;
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

	public override void _ExitTree()
	{
		MineActions.OnOneHourPassed -= PlayerFaintOnMidnight;
	}
}