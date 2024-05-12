using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Scripts.Test;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;

public partial class InventoryManager : Node2D
{
    #region Fields

    private InventoryDTO _inventoryDto;
    private HttpRequest _getInventoryHttpRequest;

    [Export] private MouseFollowingSprite _cursorFollowingSprite;
    [Export] private InventoryViewController _inventoryViewController;
    private InventorySlot[] _inventorySlots;

    #endregion
    
    #region Initializers

    public override void _EnterTree()
    {
        CreateHttpRequest();
        _inventorySlots = _inventoryViewController.InventoryItems;
        InitializeDiReferences();
        SubscribeToActions();
        GetInventory();
    }

    private void CreateHttpRequest()
    {
        _getInventoryHttpRequest = new HttpRequest();
        AddChild(_getInventoryHttpRequest);
        _getInventoryHttpRequest.RequestCompleted += OnGetInventoryHttpRequestCompleted;
    }
    private void SubscribeToActions()
    {
        MineActions.OnInventoryUpdate += SetInventoryItemsToSlotsOnInventoryOpen;
    }

    private void InitializeDiReferences()
    {
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
    }

    private void GetInventory()
    {
        var url = ApiAddress.PlayerApiPath + "GetInventory";
        _getInventoryHttpRequest.CancelRequest();
        _getInventoryHttpRequest.Request(url);
    }
    
    private void OnGetInventoryHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var inventory = JsonSerializer.Deserialize<Inventory>(jsonStr);
        _inventoryDto.Inventory = inventory;
        foreach (var item in _inventoryDto.Inventory.InventoryItems)
        {
            GD.Print(item.Variant);
        }
        GD.Print("Inventory Update from Inventory Manager");

        InitializeInventorySlots();
        SetInventorySlotUnlockStatus();
        SetInventoryItemsToSlotsOnInventoryOpen();
    }
    
    private void InitializeInventorySlots()
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
            _inventorySlots[i].SetSlotNumber(i);
    }

    private void SetInventorySlotUnlockStatus()
    {
        foreach (var slot in _inventorySlots)
            slot.SetSlotUnlockStatus(false);

        for (int i = 0; i < _inventoryDto.Inventory.SlotsUnlocked; i++)
            _inventorySlots[i].SetSlotUnlockStatus(true);
    }

    private void SetInventoryItemsToSlotsOnInventoryOpen()
    {
        for (var i = 0; i < _inventoryDto.Inventory.SlotsUnlocked; i++)
            _inventorySlots[i].ResetInventoryItemSlot();
        
        foreach (var item in _inventoryDto.Inventory.InventoryItems)
            _inventorySlots[item.Slot].SetInventoryItemToSlot(item);
    }
    
    #endregion
    
    private void RefreshInventoryOccupiedSlots()
    {
        _inventoryDto.Inventory.OccupiedSlots.Clear();
        foreach (var inventoryItem in _inventoryDto.Inventory.InventoryItems)
            _inventoryDto.Inventory.OccupiedSlots.Add(inventoryItem.Slot);
    }
    
    public bool HasFreeSlot()
    {
        var inventory = _inventoryDto.Inventory;
        return inventory.SlotsUnlocked > inventory.OccupiedSlots.Count;
        // var emptySlots = new List<int>();
        // for (var i = 0; i < _inventoryDto.Inventory.SlotsUnlocked; i++)
        //     emptySlots.Add(i);
        // foreach (var inventoryItem in _inventoryDto.Inventory.InventoryItems)
        //     emptySlots.Remove(inventoryItem.Slot);
        //
        // emptySlots.Sort();
        // return emptySlots.Any();
    }
    
    public int GetNextEmptySlot()
    {
        var emptySlots = new List<int>();
        for (var i = 0; i < _inventoryDto.Inventory.SlotsUnlocked; i++)
            emptySlots.Add(i);
        foreach (var inventoryItem in _inventoryDto.Inventory.InventoryItems)
            emptySlots.Remove(inventoryItem.Slot);
        
        emptySlots.Sort();
        return emptySlots[0];
    }
    
    #region Pick Up

    private void PickUpOneByOneSameVariantStackable(InventoryItem item)
    {
        var cursorItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
        if (cursorItem == null)
        {
            var newInventoryItem = DeepCopy(item);
            newInventoryItem.Id = Guid.NewGuid().ToString();
            newInventoryItem.Stack = 1;
            item.Stack--;
            _cursorFollowingSprite.ShowMouseFollowSprite(newInventoryItem);
        }
        else
        {
            if (cursorItem.Variant != item.Variant) return;
            if (!item.IsStackable) return;
            cursorItem.Stack++;
            item.Stack--;
            _cursorFollowingSprite.ShowMouseFollowSprite(cursorItem);
        }
    }

    private void PickUpAllSameVariantStackable(InventoryItem item)
    {
        var cursorItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
        if (cursorItem.Variant != item.Variant) return;
        if (!item.IsStackable) return;
        cursorItem.Stack += item.Stack;
        _cursorFollowingSprite.ShowMouseFollowSprite(cursorItem);
        _inventoryDto.Inventory.InventoryItems.Remove(item);
    }

    private void PickUpAllDifferentVariant(InventoryItem item)
    {
        _cursorFollowingSprite.ShowMouseFollowSprite(item);
        var itemInInventory = _inventoryDto.Inventory.InventoryItems.FirstOrDefault(i => i.Id == item.Id);
        _inventoryDto.Inventory.InventoryItems.Remove(itemInInventory);
        foreach (var inventoryItem in _inventoryDto.Inventory.InventoryItems)
        {
            GD.Print($"item {inventoryItem.Variant}, slot: {inventoryItem.Slot}");
        }
    }

    #endregion
    
    #region Deposit

    private void DepositAllSameVariantStackable(InventoryItem slotItem)
    {
        var cursorItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
        if (cursorItem.Variant != slotItem.Variant) return;
        if (!slotItem.IsStackable) return;
        slotItem.Stack += cursorItem.Stack;
        _cursorFollowingSprite.HideFollowSpriteAndSetInventoryItemToNull();
    }

    private string DepositAllDifferentVariant(int slotNo)
    {
        var cursorItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
        cursorItem.Id = Guid.NewGuid().ToString();
        cursorItem.Slot = slotNo;
        _inventoryDto.Inventory.InventoryItems.Add(cursorItem);
        _cursorFollowingSprite.HideFollowSpriteAndSetInventoryItemToNull();
        return cursorItem.Id;
    }

    #endregion
    
    private static T DeepCopy<T>(T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        
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
    
    public void MakeDecision(int slotNumber, bool isSlotEmpty, MouseButton mouseButton, out int stackNo,
        out string pngPath, out bool emptySlot)
    {
        if (!isSlotEmpty)
        {
            var item = _inventoryDto.Inventory.InventoryItems.FirstOrDefault(tempItem => tempItem.Slot == slotNumber);
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
                        var item1 = _inventoryDto.Inventory.InventoryItems.FirstOrDefault(i => i.Id == cursorItemId);
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
            else if(mouseButton == MouseButton.Right)
            {
                //RIGHT BUTTON
                var cursorItem = _cursorFollowingSprite.GetCurrentCursorInventoryItem();
                if (cursorItem == null)
                {
                    if (!item.IsStackable)
                    {
                        PickUpAllDifferentVariant(item);
                        stackNo = 0;
                        pngPath = "";
                        emptySlot = true;
                    }
                    else
                    {
                        if (item.Stack > 1)
                        {
                            PickUpOneByOneSameVariantStackable(item);
                            stackNo = item.Stack;
                            pngPath = item.PngPath;
                            emptySlot = false;
                        }
                        else
                        {
                            PickUpAllDifferentVariant(item);
                            stackNo = 0;
                            pngPath = "";
                            emptySlot = true;
                        }
                    }
                }
                else
                {
                    if (item.Variant == cursorItem.Variant)
                    {
                        if (item.Stack > 1)
                        {
                            PickUpOneByOneSameVariantStackable(item);
                            stackNo = item.Stack;
                            pngPath = item.PngPath;
                            emptySlot = false;
                        }
                        else
                        {
                            PickUpAllSameVariantStackable(item);
                            stackNo = 0;
                            pngPath = "";
                            emptySlot = true;
                        }
                    }
                    else
                    {
                        stackNo = item.Stack;
                        pngPath = item.PngPath;
                        emptySlot = false;
                    }
                }
            }
            else
            {
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
                var item1 = _inventoryDto.Inventory.InventoryItems.FirstOrDefault(tempItem => tempItem.Slot == slotNumber);
                stackNo = item1.Stack;
                pngPath = item1.PngPath;
                emptySlot = false;
            }
        }
        
        RefreshInventoryOccupiedSlots();
        MineActions.OnInventoryUpdate?.Invoke();
    }

    #region Exit Tree

    private void UnsubscribeToActions()
    {
        MineActions.OnInventoryUpdate -= SetInventoryItemsToSlotsOnInventoryOpen;
    }
    
    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }

    #endregion
}