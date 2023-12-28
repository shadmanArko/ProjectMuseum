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

	private void PlayerFaintOnMidnight(int hours)
	{
		if (hours is < 1 or > 8) return;
		_playerControllerVariables.PlayerHealth = 0;
	}
}