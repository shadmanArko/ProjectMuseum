using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
        _inventorySlots = _inventoryViewController.InventoryItems;
        InitializeDiReferences();
        SubscribeToActions();
        _isGamePausedByInventory = false;
    }
    
    private void SubscribeToActions()
    {
        MineActions.OnInventoryUpdate += SetInventoryItemsToSlotsOnInventoryOpen;
        MineActions.OnInventoryInitialized += SetUpInventoryOnInventoryDataInitialized;
    }

    private void InitializeDiReferences()
    {
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
    }
    
    // public override async void _Ready()
    // {
    //     await Task.Delay(2000);
    //     SetUpInventoryOnInventoryDataInitialized();
    // }

    private void SetUpInventoryOnInventoryDataInitialized()
    {
        InitializeInventorySlots();
        SetInventorySlotUnlockStatus();
        SetInventoryItemsToSlotsOnInventoryOpen();
    }
    
    private void InitializeInventorySlots()
    {
        for (var i = 0; i < _inventorySlots.Length; i++)
            _inventorySlots[i].SetSlotNumber(i);
    }

    private void SetInventorySlotUnlockStatus()
    {
        foreach (var slot in _inventorySlots)
            slot.SetSlotUnlockStatus(false);

        for (var i = 0; i < _inventoryDto.Inventory.SlotsUnlocked; i++)
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

    #region Input

    private bool _isGamePausedByInventory;
    public override void _Input(InputEvent inputEvent)
    {
        if (!inputEvent.IsActionReleased("Inventory")) return;
        
        if (ReferenceStorage.Instance.MinePauseManager.IsPaused)
        {
            GD.Print($"isPaused: {ReferenceStorage.Instance.MinePauseManager.IsPaused}");
            if (_isGamePausedByInventory)
            {
                if (HasItemAtHand())
                {
                    GD.Print($"has item in hand: {HasItemAtHand()}");
                    return;
                }
                GD.Print("UNPAUSING");
                MineActions.OnGameUnpaused?.Invoke();
                ReferenceStorage.Instance.InventoryViewController.HideInventory();
                ReferenceStorage.Instance.MineUiController.ShowToolbarPanel();
                _isGamePausedByInventory = false;
            }
            
        }
        else
        {
            GD.Print("PAUSING");
            MineActions.OnGamePaused?.Invoke();
            ReferenceStorage.Instance.InventoryViewController.ShowInventory();
            ReferenceStorage.Instance.MineUiController.HideToolbarPanel();
            _isGamePausedByInventory = true;
        }
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

    #region Utilities

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

    public bool HasItemAtHand() => _cursorFollowingSprite.GetCurrentCursorInventoryItem() != null;

    #endregion
    
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
        MineActions.OnInventoryInitialized -= SetUpInventoryOnInventoryDataInitialized;
    }
    
    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }

    #endregion
}