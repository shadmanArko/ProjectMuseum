using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.Types.Consumable;

public partial class EnergyPotion : Node2D, IConsumable
{
    private PlayerControllerVariables _playerControllerVariables;

    private int _healthBoost;

    #region Initializer

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

    #endregion
    public bool CheckEligibility()
    {
        var playerEnergy = _playerControllerVariables.PlayerEnergy;
        return playerEnergy < 200;
    }

    public void ApplyStatEffect()
    {
        IncreasePlayerEnergy();
    }
    
    private async void IncreasePlayerEnergy()
    {
        var eligibility = CheckEligibility();
        if (eligibility)
        {
            EnergySystem.RestoreEnergy(50, _playerControllerVariables);
            ReferenceStorage.Instance.MinePopUp.ShowPopUp("Energy increased by 50");
        }
        else
        {
            ReferenceStorage.Instance.MinePopUp.ShowPopUp("Energy is already full");
        }
    }

}