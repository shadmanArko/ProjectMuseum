using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class OperationControlManager : Node2D
{
    [Export] private WallPlaceableController _wallPlaceableController;
    

    private Inventory _inventory;
    private int _slot;
    
    private HttpRequest _getPlayerInventoryHttpRequest;
    private HttpRequest _getWallPlaceableByVariantHttpRequest;

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
        
        _getWallPlaceableByVariantHttpRequest = new HttpRequest();
        AddChild(_getWallPlaceableByVariantHttpRequest);
        _getWallPlaceableByVariantHttpRequest.RequestCompleted += OnGetWallPlaceableByVariantHttpRequestComplete;
    }

    private void SubscribeToActions()
    {
        MineActions.OnToolbarSlotChanged += ActivateControllerBasedInItemType;
    }

    #endregion

    private void ActivateControllerBasedInItemType(int slot)
    {
        _slot = slot;
        GetPlayerInventory();
    }

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
        
        GD.Print("get player inventory request completed");
        
        if (_inventory == null)
        {
            GD.Print("INVENTORY IS NULL");
            return;
        }
        
        GD.Print($"inventory item count = {_inventory.InventoryItems.Count}");
        if (_inventory.InventoryItems.Count > 0)
        {
            foreach (var item in _inventory.InventoryItems)
            {
                GD.Print(item.Variant);
            }
        }
        else
        {
            GD.Print($"inventory item count is 0");
        }

        var inventoryItem = _inventory.InventoryItems.FirstOrDefault(tempItem => tempItem.Slot == _slot);
        
        if (inventoryItem == null)
        {
            GD.Print("inventory item is null");
        }
        else
        {
            GD.Print("inventory item not null");
            ActivateController(inventoryItem);
            // if (inventoryItem.Type == "WallPlaceable")
            // {
            //     GD.Print($"inventory item is not null {inventoryItem.Variant}");
            //     _wallPlaceableController.OnSelectWallPlaceableFromInventory(inventoryItem);
            // }
        }
    }
    
    #endregion

    #region Get Wall Placeable API

    private void GetWallPlaceableByVariant(string variant)
    {
        var url = ApiAddress.MineApiPath+"GetWallPlaceableByVariant/"+variant;
        _getWallPlaceableByVariantHttpRequest.Request(url);
    }
    
    private void OnGetWallPlaceableByVariantHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var wallPlaceable = JsonSerializer.Deserialize<WallPlaceable>(jsonStr);
        
        // _wallPlaceableController.OnSelectWallPlaceableFromInventory(GetInventoryItem());
    }

    #endregion

    private void ActivateController(InventoryItem inventoryItem)
    {
        switch (inventoryItem.Type)
        {
            case "WallPlaceable":
                _wallPlaceableController.OnSelectWallPlaceableFromInventory(inventoryItem);
                break;
        }
    }
}