using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Scripts.Test;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;

public partial class InventoryManager : Node2D
{
    private Inventory _inventory;

    private HttpRequest _getInventoryHttpRequest;

    [Export] private MouseFollowingSprite _cursorFollowingSprite;
    [Export] private InventorySlot[] _inventorySlots;


    #region Initializers

    public override void _EnterTree()
    {
        CreateHttpRequest();
    }

    public override void _Ready()
    {
        // _inventorySlots = new InventorySlot[12];
        InitializeDiReferences();
        GetInventory();
    }

    private void InitializeDiReferences()
    {
        _inventory = ServiceRegistry.Resolve<Inventory>();
    }

    private void CreateHttpRequest()
    {
        _getInventoryHttpRequest = new HttpRequest();
        AddChild(_getInventoryHttpRequest);
        _getInventoryHttpRequest.RequestCompleted += OnGetInventoryHttpRequestCompleted;
    }

    #endregion

    #region Get Inventory Http Request

    private void GetInventory()
    {
        var url = ApiAddress.PlayerApiPath + "GetInventory";
        _getInventoryHttpRequest.CancelRequest();
        _getInventoryHttpRequest.Request(url);
    }

    private void OnGetInventoryHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        _inventory = JsonSerializer.Deserialize<Inventory>(jsonStr);
        foreach (var item in _inventory.InventoryItems)
        {
            GD.Print(item.Variant);
        }

        InitializeInventorySlots();
        SetInventoryItemsToSlotsOnInventoryOpen();
    }

    private void InitializeInventorySlots()
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
            _inventorySlots[i].SetSlotNumber(i);
    }

    private void SetInventoryItemsToSlotsOnInventoryOpen()
    {
        foreach (var item in _inventory.InventoryItems)
            _inventorySlots[item.Slot].SetInventoryItemToSlot(item);
    }

    #endregion

    public InventoryItem SetItemFromInventorySlotToMouseCursor(InventoryItem inventoryItem)
    {
        var cursorInventoryItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
        _cursorFollowingSprite.ShowMouseFollowSprite(inventoryItem);
        var isRemoved = _inventory.InventoryItems.Remove(inventoryItem);
        foreach (var item in _inventory.InventoryItems)
        {
            GD.Print(item.Variant);
        }

        MineActions.OnInventoryUpdate?.Invoke();

        return cursorInventoryItem;
    }

    #region Pick Up

    public void PickUpOneByOneSameVariantStackable(InventoryItem item)
    {
        
    }

    public void PickUpAllSameVariantStackable(InventoryItem item)
    {
        var cursorItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
        if (cursorItem.Variant != item.Variant) return;
        if (!item.IsStackable) return;
        cursorItem.Stack += item.Stack;
        _cursorFollowingSprite.ShowMouseFollowSprite(cursorItem);
        _inventory.InventoryItems.Remove(item);
    }

    public void PickUpAllDifferentVariant(InventoryItem item)
    {
        _cursorFollowingSprite.ShowMouseFollowSprite(item);
        var itemInInventory = _inventory.InventoryItems.FirstOrDefault(i => i.Id == item.Id);
        _inventory.InventoryItems.Remove(itemInInventory);
        foreach (var inventoryItem in _inventory.InventoryItems)
        {
            GD.Print($"item {inventoryItem.Variant}, slot: {inventoryItem.Slot}");
        }
    }

    #endregion

    #region Deposit

    public void DepositAllSameVariantStackable(InventoryItem slotItem)
    {
        var cursorItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
        if (cursorItem.Variant != slotItem.Variant) return;
        if (!slotItem.IsStackable) return;
        slotItem.Stack += cursorItem.Stack;
        _cursorFollowingSprite.HideFollowSpriteAndSetInventoryItemToNull();
    }

    public string DepositAllDifferentVariant(int slotNo)
    {
        var cursorItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
        cursorItem.Id = Guid.NewGuid().ToString();
        cursorItem.Slot = slotNo;
        _inventory.InventoryItems.Add(cursorItem);
        _cursorFollowingSprite.HideFollowSpriteAndSetInventoryItemToNull();
        return cursorItem.Id;
    }

    #endregion

    public void MakeDecision(int slotNumber, bool isSlotEmpty, MouseButton mouseButton, out int stackNo,
        out string pngPath, out bool emptySlot)
    {
        if (!isSlotEmpty)
        {
            var item = _inventory.InventoryItems.FirstOrDefault(tempItem => tempItem.Slot == slotNumber);
            var tempItem = DeepCopy(item);

            if (mouseButton == MouseButton.Left)
            {
                if (_cursorFollowingSprite.GetCurrentCursorInventoryItem() != null)
                {
                    var cursorItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
                    if (cursorItem.Variant == item.Variant)
                    {
                        //ADD TO STACK
                        DepositAllSameVariantStackable(item);
                        stackNo = item.Stack;
                        pngPath = item.PngPath;
                        emptySlot = false;
                    }
                    else
                    {
                        //SWAP
                        
                        var cursorItemId = DepositAllDifferentVariant(slotNumber);
                        var item1 = _inventory.InventoryItems.FirstOrDefault(i => i.Id == cursorItemId);
                        stackNo = item1.Stack;
                        pngPath = item1.PngPath;
                        emptySlot = false;
                        PickUpAllDifferentVariant(tempItem);
                    }
                }
                else
                {
                    //FROM SLOT TO CURSOR
                    PickUpAllDifferentVariant(item);
                    stackNo = 0;
                    pngPath = "";
                    emptySlot = true;
                }
            }
            else
            {
                //RIGHT BUTTON
                stackNo = 0;
                pngPath = "";
                emptySlot = true;
            }
        }
        else
        {
            var cursorItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
            if (cursorItem == null)
            {
                stackNo = 0;
                pngPath = "";
                emptySlot = true;
            }
            else
            {
                DepositAllDifferentVariant(slotNumber);
                var item1 = _inventory.InventoryItems.FirstOrDefault(tempItem => tempItem.Slot == slotNumber);
                stackNo = item1.Stack;
                pngPath = item1.PngPath;
                emptySlot = false;
            }
        }
    }

    public static T DeepCopy<T>(T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        // Using System.Text.Json for serialization and deserialization
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IgnoreNullValues = true
        };

        // Serialize the object to a JSON string
        string jsonString = JsonSerializer.Serialize(obj, options);

        // Deserialize the JSON string to a new instance
        T copy = JsonSerializer.Deserialize<T>(jsonString, options);

        return copy;
    }
}