using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.Types.Consumable;

public partial class HealthPotion : Node2D, IConsumable
{
	private PlayerControllerVariables _playerControllerVariables;

	private int _healthBoost;
	
	public override void _EnterTree()
	{
		InitializeDiInstaller();
	}

	public override void _Ready()
	{
		_healthBoost = 50;
	}

	private void InitializeDiInstaller()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}

	public bool CheckEligibility()
	{
		var playerHealth = _playerControllerVariables.PlayerHealth;
		return playerHealth < 200;
	}

	public void ApplyStatEffect()
	{
		IncreasePlayerHealth();
	}
	
	private async void IncreasePlayerHealth()
	{
		var eligibility = CheckEligibility();
		if (eligibility)
		{
			HealthSystem.RestorePlayerHealth(50, _playerControllerVariables);
			await ReferenceStorage.Instance.MinePopUp.ShowPopUp("Health increased by 50 Hp");
		}
		else
		{
			await ReferenceStorage.Instance.MinePopUp.ShowPopUp("Health is already full");
		}
	}
}