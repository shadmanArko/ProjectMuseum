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
    }

    public override void DeactivateController()
    {
        if(!IsControllerActivated) return;
        IsControllerActivated = false;
        UnsubscribeToActions();
        MineActions.OnInventoryUpdate?.Invoke();
    }

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
                // MineActions.OnMeleeAttackActionEnded -= MeleeActionEnd;
                break;
            case "Ranged":
                break;
        }
        
        MineActions.DeselectAllInventoryControllers -= DeactivateController;
    }

    private void MeleeActionStart()
    {
        if(_playerControllerVariables.IsAttacking) return;
        GD.Print("inside melee action started");
        if(!_playerControllerVariables.CanAttack) return;
        _playerControllerVariables.IsAttacking = true;
        MineActions.OnMeleeAttackActionStarted?.Invoke();
        MuseumActions.OnPlayerPerformedTutorialRequiringAction.Invoke("AttackActionCompleted");
    }

    private void DigActionStart()
    {
        GD.Print("inside dig action started");
        SetProcess(true);
    }
    
    private void DigActionEnd()
    {
        SetProcess(false);
    }
    
    public override void _Process(double delta)
    {
        if (_playerControllerVariables.CanDig)
        {
            MineActions.OnDigActionStarted?.Invoke();
            GD.Print("is digging");
            MuseumActions.OnPlayerPerformedTutorialRequiringAction.Invoke("DigActionCompleted");
        }
    }

    #endregion
}