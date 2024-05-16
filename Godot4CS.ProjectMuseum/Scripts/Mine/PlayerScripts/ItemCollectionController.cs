using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;
using Godot4CS.ProjectMuseum.Scripts.Mine.Objects;
using ProjectMuseum.DTOs;

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

        var inventoryItem = item.GetItem();
        var inventoryStack =
            _inventoryDto.Inventory.InventoryItems.FirstOrDefault(item1 => item1.Variant == inventoryItem.Variant);
        if (inventoryItem.IsStackable && inventoryStack != null)
        {
            
        }
        if(inventoryItem.IsStackable && inventoryStack == null) return;
        item.SetCollisionLayerValue(4, true);
        item.SetCollisionLayerValue(6, false);
        // if (!_inventoryManager.HasFreeSlot())
        // {
        //     // item.Sleeping
        //     return;
        // }
        // GD.Print($"PULLING ITEM TOWARDS PLAYER {item.GetItem().Variant}");
        // item.Sleeping = false;
    }
    
    private void OnBodyExited(Node2D body)
    {
        var item = body as ItemDrop;
        if(item == null) return;
        GD.Print($"NOT PULLING ITEM TOWARDS PLAYER {item.GetItem().Variant}");
        item.Sleeping = true;
    }

    #endregion

    #region Item Stasher

    private void OnBodyEnterStashArea(Node2D body)
    {
        GD.Print($"body name: {body.Name}");
        var item = body as ItemDrop;
        if(item == null) return;
        var inventoryItem = item.GetItem();
        
        if(!_inventoryManager.HasFreeSlot() && !inventoryItem.IsStackable) return;
        if (inventoryItem == null)
        {
            GD.PrintErr($"inventory item is null");
            return;
        }
        
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