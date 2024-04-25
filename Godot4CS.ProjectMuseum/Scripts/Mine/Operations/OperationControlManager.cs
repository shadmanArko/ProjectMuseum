using System.Linq;
using Godot;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class OperationControlManager : Node2D
{
    private HttpRequest _getPlayerInventoryHttpRequest;

    [Export] private InventoryControllers.InventoryController[] _inventoryControllers;
    
    [Export] private InventoryControllers.WallPlaceableController _wallPlaceableController;
    [Export] private InventoryControllers.ConsumableController _consumableController;
    [Export] private InventoryControllers.EquipableController _equipableController;
    
    private Inventory _inventory;
    private int _slot;
    
    #region Initializers

    public override void _Ready()
    {
        CreateHttpRequest();
        SubscribeToActions();
    }

    private void CreateHttpRequest()
    {
        _getPlayerInventoryHttpRequest = new HttpRequest();
        AddChild(_getPlayerInventoryHttpRequest);
        _getPlayerInventoryHttpRequest.RequestCompleted += OnGetPlayerInventoryHttpRequestComplete;
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
        GetPlayerInventory();
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

    private void GetPlayerInventory()
    {
        var url = ApiAddress.PlayerApiPath+"GetInventory";
        _getPlayerInventoryHttpRequest.CancelRequest();
        _getPlayerInventoryHttpRequest.Request(url);
        GD.Print("Get Player inventory called");
    }
	   
    private void OnGetPlayerInventoryHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        _inventory = JsonSerializer.Deserialize<Inventory>(jsonStr);
        
        if (_inventory == null)
        {
            GD.Print("INVENTORY IS NULL");
            return;
        }
        
        GD.Print($"inventory item count = {_inventory.InventoryItems.Count}");
        if (_inventory.InventoryItems.Count <= 0)
        {
            GD.Print("Inventory empty");
            return;
        }

        var inventoryItem = _inventory.InventoryItems.FirstOrDefault(tempItem => tempItem.Slot == _slot);
        if (inventoryItem == null)
        {
            GD.Print("inventory item is NULL");
            return;
        }
        ActivateRequiredController(inventoryItem);
    }
    
    private void ActivateRequiredController(InventoryItem inventoryItem)
    {
        switch (inventoryItem.Type)
        {
            case "WallPlaceable":
                _wallPlaceableController.OnSelectWallPlaceableFromInventory(inventoryItem);
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