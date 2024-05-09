using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations.InventoryControllers;

public partial class EquipableController : InventoryController
    {
    private PlayerControllerVariables _playerControllerVariables;
    
    private InventoryItem _inventoryItem;
    
    #region Initializers

    public override void _Ready()
    {
        InitializeDiReference();
        SetProcess(false);
    }

    private void InitializeDiReference()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }

    public override void ActivateController(InventoryItem inventoryItem)
    {
        IsControllerActivated = true;
        _inventoryItem = inventoryItem;
        SubscribeToActions();
        GD.Print("Equipable controller activated");
        GD.Print(inventoryItem.Variant);
    }

    public override void DeactivateController()
    {
        if(!IsControllerActivated) return;
        IsControllerActivated = false;
        MineActions.OnLeftMouseHeldActionEnded?.Invoke();
        UnsubscribeToActions();
        MineActions.OnInventoryUpdate?.Invoke();
    }

    #region Subscribe and Unsubscribe to Actions

    private void SubscribeToActions()
    {
        switch (_inventoryItem.Category)
        {
            case "Pickaxe":
                MineActions.OnLeftMouseHeldActionStarted += DigActionStart;
                MineActions.OnLeftMouseHeldActionEnded += DigActionEnd;
                GD.Print("subscribed to pickaxe actions");
                break;
            case "Melee":
                MineActions.OnLeftMouseClickAction += MeleeActionStart;
                GD.Print("subscribed to melee actions");
                break;
            case "Ranged":
                break;
        }
        
        MineActions.DeselectAllInventoryControllers += DeactivateController;
    }

    private void UnsubscribeToActions()
    {
        switch (_inventoryItem.Category)
        {
            case "Pickaxe":
                MineActions.OnLeftMouseHeldActionStarted -= DigActionStart;
                MineActions.OnLeftMouseHeldActionEnded -= DigActionEnd;
                break;
            case "Melee":
                MineActions.OnLeftMouseClickAction -= MeleeActionStart;
                break;
            case "Ranged":
                break;
        }
        
        MineActions.DeselectAllInventoryControllers -= DeactivateController;
    }

    #endregion

    #region Melee Attack

    private void MeleeActionStart()
    {
        if(!_playerControllerVariables.CanAttack) return;
        MineActions.OnMeleeAttackActionStarted?.Invoke();
        MuseumActions.OnPlayerPerformedTutorialRequiringAction.Invoke("AttackActionCompleted");
    }

    #endregion

    #region Dig Start and End

    private void DigActionStart()
    {
        _playerControllerVariables.IsDigging = true;
        GD.Print("inside dig action started");
        SetProcess(true);
    }
    
    private void DigActionEnd()
    {
        SetProcess(false);
        _playerControllerVariables.IsDigging = false;
    }

    #endregion
    
    public override void _Process(double delta)
    {
        if (_playerControllerVariables.CanDig && _playerControllerVariables.PlayerEnergy > 0)
        {
            _playerControllerVariables.Velocity = new Vector2(0, _playerControllerVariables.Velocity.Y);
            MineActions.OnDigActionStarted?.Invoke();
            MuseumActions.OnPlayerPerformedTutorialRequiringAction.Invoke("DigActionCompleted");
        }
        else
            DigActionEnd();
    }

    #endregion
}