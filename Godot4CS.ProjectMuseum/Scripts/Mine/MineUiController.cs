using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineUiController : CanvasLayer
{
	[Export] private TextureProgressBar _healthBar;
	[Export] private TextureProgressBar _energyBar;
	[Export] private Label _playerCoordinates;
	
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;

	#region Initializers

	public override void _Ready()
	{
		InitializeDiReferences();
		SubscribeToActions();
	}

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerHealthValueChanged += UpdateHealthBar;
		MineActions.OnPlayerEnergyValueChanged += UpdateEnergyBar;
		MineActions.OnPlayerPositionUpdated += SetPlayerCoordinates;
	}

	#endregion
	
	private void UpdateHealthBar()
	{
		_healthBar.Value = _playerControllerVariables.PlayerHealth;
	}

	private void UpdateEnergyBar()
	{
		_energyBar.Value = _playerControllerVariables.PlayerEnergy;
	}

	private void SetPlayerCoordinates()
	{
		var currentPos = _playerControllerVariables.Position;
		var tilePos = _mineGenerationVariables.MineGenView.TileMap.LocalToMap(currentPos);
		var str = $"(X,Y) = ({tilePos.X},{tilePos.Y})";
		_playerCoordinates.Text = tilePos is { X: > 0, Y: > 0 } ? str : "";
	}

	#region Exit

	private void UnsubscribeToActions()
	{
		MineActions.OnPlayerHealthValueChanged -= UpdateHealthBar;
		MineActions.OnPlayerEnergyValueChanged -= UpdateEnergyBar;
		MineActions.OnPlayerPositionUpdated -= SetPlayerCoordinates;
	}

	public override void _ExitTree()
	{
		UnsubscribeToActions();
	}

	#endregion
}