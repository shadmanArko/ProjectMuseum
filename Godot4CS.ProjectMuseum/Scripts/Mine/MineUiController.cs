using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineUiController : CanvasLayer
{
	[Export] private TextureProgressBar _healthBar;
	[Export] private TextureProgressBar _energyBar;
	
	private PlayerControllerVariables _playerControllerVariables;

	public override void _EnterTree()
	{
		InitializeDiReferences();
		SubscribeToActions();
	}
	
	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerHealthValueChanged += UpdateHealthBar;
		MineActions.OnPlayerEnergyValueChanged += UpdateEnergyBar;
	}
	
	private void UpdateHealthBar()
	{
		_healthBar.Value = _playerControllerVariables.PlayerHealth;
	}

	private void UpdateEnergyBar()
	{
		_energyBar.Value = _playerControllerVariables.PlayerEnergy;
	}
    
}