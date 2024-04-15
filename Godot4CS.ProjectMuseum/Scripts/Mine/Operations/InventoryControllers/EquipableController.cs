using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations.InventoryControllers;

public partial class EquipableController : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    
    #region Initializers

    public override void _Ready()
    {
        InitializeDiReference();
    }

    private void InitializeDiReference()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }

    public void OnSelectEquipable(InventoryItem inventoryItem)
    {
        SubscribeToActions();
    }
    
    private void SubscribeToActions()
    {
        MineActions.OnLeftMouseClickActionStarted += SwingPickaxe;
    }

    private void UnsubscribeToActions()
    {
        MineActions.OnLeftMouseClickActionStarted -= SwingPickaxe;
    }

    private void SwingPickaxe()
    {
        SetProcess(true);
    }
    
    public override void _Process(double delta)
    {
        MineActions.OnDigActionStarted?.Invoke();
    }

    #endregion
}