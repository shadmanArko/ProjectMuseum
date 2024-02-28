using System.Collections.Generic;
using Godot;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class OperationControlManager : Node2D
{
    [Export] private WallPlaceableController _wallPlaceableController;
    

    private Inventory _inventory = new();
    private int _slot;
    
    private HttpRequest _getPlayerInventoryHttpRequest;
    private HttpRequest _getWallPlaceableByVariantHttpRequest;

    #region Initializers

    public override void _Ready()
    {
        _inventory.InventoryItems = new List<InventoryItem>();
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
        // GetPlayerInventory();
    }

    #region Get Inventory

    private void GetPlayerInventory()
    {
        var url = ApiAddress.PlayerApiPath+"GetInventory";
        _getPlayerInventoryHttpRequest.CancelRequest();
        _getPlayerInventoryHttpRequest.Request(url);
    }
	   
    private void OnGetPlayerInventoryHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        _inventory = JsonSerializer.Deserialize<Inventory>(jsonStr);

        if(_inventory == null)
            GD.Print("INVENTORY IS NULL");
        else
        {
            var inventoryItem = _inventory.InventoryItems[_slot];
            if(inventoryItem == null)
                GD.Print("inventoryItem is null");
            else
            {
                if(inventoryItem.Type == "WallPlaceable")
                    _wallPlaceableController.OnSelectWallPlaceableFromInventory(inventoryItem);
            }
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
                GetWallPlaceableByVariant(inventoryItem.Variant);
                break;
        }
    }

    private InventoryItem GetInventoryItem()
    {
        if (_inventory == null)
        {
            GD.PrintErr("Inventory is null");
            return null;
        }

        var inventoryItem = _inventory.InventoryItems[5];
        GD.PrintErr($"inventory item is null: {inventoryItem == null}");

        return inventoryItem;
    }

    

    #region Input Test

    private const string TorchScenePath = "res://Scenes/Mine/Sub Scenes/Props/FireTorch.tscn";
    
    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionReleased("ui_wheel_up"))
        {
            GD.Print("Mouse wheel up");
        }
        
        if (inputEvent.IsActionReleased("ui_wheel_down"))
        {
            GD.Print("Mouse wheel down");
        }
        
        if (inputEvent.IsActionReleased("Lamp"))
        {
            // _wallPlaceableController.OnSelectWallPlaceableFromInventory(TorchScenePath);
            // GetWallPlaceableByVariant("FireTorch");
            GetPlayerInventory();
        }
        
        if(inputEvent.IsActionReleased("ui_left_click"))
            MineActions.OnLeftMouseClickActionEnded?.Invoke();
        
        if(inputEvent.IsActionReleased("ui_right_click"))
            MineActions.OnRightMouseClickActionEnded?.Invoke();
    }

    #endregion
}