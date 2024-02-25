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
    }
	
    private void OnGetPlayerInventoryHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        _inventory = JsonSerializer.Deserialize<Inventory>(jsonStr);
        var inventoryItem = GetInventoryItem();
        ActivateController(inventoryItem);
    }

    private void ActivateController(InventoryItem inventoryItem)
    {
        // if(inventoryItem.Type == "WallPlaceable")
        //     _wallPlaceableController.OnSelectWallPlaceableFromInventory();
    }

    private InventoryItem GetInventoryItem()
    {
        if (_inventory == null)
        {
            GD.PrintErr("Inventory is null");
            return null;
        }

        var inventoryItem = _inventory.InventoryItems[_slot];
        GD.PrintErr($"inventory item is null: {inventoryItem == null}");

        return inventoryItem;
    }

    #endregion

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
            _wallPlaceableController.OnSelectWallPlaceableFromInventory(TorchScenePath);
        }
        if(inputEvent.IsActionReleased("ui_right_click"))
            MineActions.OnRightMouseClickActionEnded?.Invoke();
    }

    #endregion
}