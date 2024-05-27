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

    public override void _Ready()
    {
        InitializeDiInstaller();
        _inventoryManager = ReferenceStorage.Instance.InventoryManager;
    }

    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
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
        // var inventory = _inventoryDto.Inventory;
        var inventoryItem = item.InventoryItem;
        
        item.SetPhysicsProcess(true);
        if (inventoryItem.IsStackable)
        {
            AddItemAsStackable(item);
        }
        else
        {
            AddItemAsNonStackable(item);
        }
    }

    private void AddItemAsStackable(ItemDrop item)
    {
        var inventoryItem = item.InventoryItem;
        var inventory = _inventoryDto.Inventory;
        var inventoryStack =
            inventory.InventoryItems.FirstOrDefault(item1 => item1.Variant == inventoryItem.Variant);
        if (inventoryStack != null)
        {
            item.SetPhysicsProcess(true);
            GD.Print("Added item to stack");
        }
        else
        {
            AddItemAsNonStackable(item);
        }
        
    }

    private void AddItemAsNonStackable(ItemDrop item)
    {
        var inventoryItem = item.InventoryItem;
        var inventory = _inventoryDto.Inventory;
        if (_inventoryManager.HasFreeSlot())
        {
            item.SetPhysicsProcess(true);
        }
        else
        {
            item.SetPhysicsProcess(false);
            GD.Print("inventory has no empty slot");
        }
    }
    
    private void OnBodyExited(Node2D body)
    {
        var item = body as ItemDrop;
        if(item == null) return;
        GD.Print($"NOT PULLING ITEM TOWARDS PLAYER {item.InventoryItem.Variant}");
        item.Sleeping = true;
    }

    #endregion

    #region Item Stasher

    private void OnBodyEnterStashArea(Node2D body)
    {
        GD.Print($"body name: {body.Name}");
        var item = body as ItemDrop;
        if(item == null) return;
        var inventoryItem = item.InventoryItem;
        
        if(!_inventoryManager.HasFreeSlot() && !inventoryItem.IsStackable) return;
        if (inventoryItem == null)
        {
            GD.PrintErr($"inventory item is null");
            return;
        }

        if (inventoryItem.Type == "Artifact")
            _inventoryDto.Inventory.Artifacts.Add(item.Artifact);
        
        MineActions.OnCollectItemDrop?.Invoke(inventoryItem);
        GD.Print("Collecting inventory item");
        item.QueueFree();
    }

    private void OnBodyExitStashArea(Node2D body)
    {
        
    }

    #endregion

    public override void _ExitTree()
    {
        
    }
}