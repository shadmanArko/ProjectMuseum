using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;
using Godot4CS.ProjectMuseum.Scripts.Mine.Objects;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class ItemCollectionController : Area2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private InventoryDTO _inventoryDto;
    private InventoryManager _inventoryManager;

    private List<ItemDrop> _itemDrops; 
    private const int ItemStackLimit = 99;

    #region Initializers

    public override void _Ready()
    {
        InitializeDiInstaller();
        _inventoryManager = ReferenceStorage.Instance.InventoryManager;
        _itemDrops = new List<ItemDrop>();
    }

    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
    }

    #endregion

    public override void _Process(double delta)
    {
        CheckIfDroppedItemsCanBeCollectable();
    }

    private void CheckIfDroppedItemsCanBeCollectable()
    {
        if (_itemDrops.Count <= 0)
        {
            GD.Print("no item inside item drop");
            SetProcess(false);
            return;
        }
        foreach (var drop in _itemDrops)
        {
            if(drop == null) continue;
            var canBeCollected = CanItemBeCollected(drop.InventoryItem);
            GD.Print($"{drop.InventoryItem.Variant} can be collected {canBeCollected}");
            drop.SetPhysicsProcess(canBeCollected);
        }
    }

    #region Item Collection Controller

    private void OnBodyEntered(Node2D body)
    {
        GD.Print($"body name: {body.Name}");
        var item = body as ItemDrop;
        if (item == null)
        {
            GD.Print("ITEM IS NOT ITEM DROP");
            return;
        }
        
        GD.Print("Item inside collection area");
        var inventoryItem = item.InventoryItem;
        if (inventoryItem == null)
        {
            GD.PrintErr("Fatal error: Inventory item is null");
            return;
        }

        var canBeCollected = CanItemBeCollected(inventoryItem);
        if (!canBeCollected)
        {
            GD.Print("CANNOT BE COLLECTED");
            if(!_itemDrops.Contains(item))
                _itemDrops.Add(item);
        }
        else
            item.SetPhysicsProcess(true);
        
        SetProcess(_itemDrops.Count >= 1);
    }
    
    private void OnBodyExited(Node2D body)
    {
        var item = body as ItemDrop;
        if(item == null) return;
        GD.Print($"NOT PULLING ITEM TOWARDS PLAYER {item.InventoryItem.Variant}");

        _itemDrops.Remove(item);
        item.SetPhysicsProcess(false);
    }

    #endregion

    #region Item Stasher

    private void OnBodyEnterStashArea(Node2D body)
    {
        GD.Print($"body name: {body.Name}");
        var item = body as ItemDrop;
        if(item == null) return;
        
        var inventoryItem = item.InventoryItem;
        if (inventoryItem == null)
        {
            GD.PrintErr($"inventory item is null");
            return;
        }

        var canBeCollected = CanItemBeCollected(inventoryItem);
        if (!canBeCollected)
        {
            GD.Print("CANNOT BE STASHED");
            item.SetPhysicsProcess(false);
            return;
        }

        if (inventoryItem.Type == "Artifact")
            _inventoryDto.Inventory.Artifacts.Add(item.Artifact);
        
        MineActions.OnCollectItemDrop?.Invoke(inventoryItem);
        GD.Print("Collecting inventory item");
        _itemDrops.Remove(item);
        item.QueueFree();
    }

    

    private void OnBodyExitStashArea(Node2D body)
    {
        
    }

    #endregion

    #region Utilities

    private bool CanItemBeCollected(InventoryItem inventoryItem)
    {
        var canBeCollected = _inventoryManager.HasFreeSlot();
        if (!inventoryItem.IsStackable) return canBeCollected;
        foreach (var tempItem in _inventoryDto.Inventory.InventoryItems)
        {
            if (tempItem.Variant != inventoryItem.Variant) continue;
            canBeCollected = canBeCollected || tempItem.Stack < ItemStackLimit;
        }

        return canBeCollected;
    }

    #endregion

    public override void _ExitTree()
    {
        
    }
}