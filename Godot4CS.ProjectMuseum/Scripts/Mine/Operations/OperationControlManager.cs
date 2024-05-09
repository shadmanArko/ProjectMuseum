using System.Linq;
using Godot;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class OperationControlManager : Node2D
{
    private HttpRequest _getPlayerInventoryHttpRequest;

    [Export] private InventoryControllers.InventoryController[] _inventoryControllers;
    
    [Export] private InventoryControllers.WallPlaceableController _wallPlaceableController;
    [Export] private InventoryControllers.ConsumableController _consumableController;
    [Export] private InventoryControllers.EquipableController _equipableController;
    
    private InventoryDTO _inventoryDto;
    private int _slot;
    
    #region Initializers

    public override void _Ready()
    {
        SubscribeToActions();
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
    }

    private void SubscribeToActions()
    {
        MineActions.OnToolbarSlotChanged += ActivateControllerBasedInItemType;
        MineActions.DeselectAllInventoryControllers += DeactivateActiveController;
    }

    private void UnsubscribeToActions()
    {
        MineActions.OnToolbarSlotChanged -= ActivateControllerBasedInItemType;
        MineActions.DeselectAllInventoryControllers -= DeactivateActiveController;
    }

    #endregion

    #region Controller Activation

    private void ActivateControllerBasedInItemType(int slot)
    {
        if(_slot == slot) return;
        _slot = slot;
        var inventoryItem = _inventoryDto.Inventory.InventoryItems.FirstOrDefault(tempItem => tempItem.Slot == _slot);
        if (inventoryItem == null)
        {
            GD.Print("inventory item is NULL");
            return;
        }
        ActivateRequiredController(inventoryItem);
    }
    
    private void DeactivateActiveController()
    {
        foreach (var controller in _inventoryControllers)
        {
            if(controller.IsControllerActivated)
                controller.DeactivateController();
        }
    }

    #endregion

    #region Get Inventory
    
    private void ActivateRequiredController(InventoryItem inventoryItem)
    {
        switch (inventoryItem.Type)
        {
            case "WallPlaceable":
                _wallPlaceableController.ActivateController(inventoryItem);
                break;
            case "Consumable":
                _consumableController.ActivateController(inventoryItem);
                break;
            case "Equipable":
                GD.Print("Selected equipable controller");
                _equipableController.ActivateController(inventoryItem);
                break;
        }
    }
    
    #endregion
}