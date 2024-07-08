using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations.InventoryControllers;

public partial class InventoryItemCollector : Node
{
    private MineGenerationVariables _mineGenerationVariables;
	
    private RandomNumberGenerator _randomNumberGenerator;
	
    private InventoryDTO _inventoryDto;
    
    #region Initializers

    private void InitializeDiInstaller()
    {
        ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
    }

    private void SubscribeToActions()
    {
        MineActions.OnCollectItemDrop += SendInventoryItemToInventory;
    }
    
    public override void _Ready()
    {
        InitializeDiInstaller();
        SubscribeToActions();
        _randomNumberGenerator = new RandomNumberGenerator();
    }

    #endregion


    private void SendInventoryItemToInventory(InventoryItem item)
    {
        var inventoryManager = ReferenceStorage.Instance.InventoryManager;
        var mine = _mineGenerationVariables.Mine;
        
        switch (item.Type)
        {
            case "Resource":
                var resources = _mineGenerationVariables.Mine.Resources;
                var resourceToRemove = resources.FirstOrDefault(res => res.Id == item.Id);
                if (resourceToRemove != null)
                    resources.Remove(resourceToRemove);
                else
                {
                    GD.PrintErr("Resource is null");
                    return;
                }

                var resourceCellPos = new Vector2I(resourceToRemove.PositionX, resourceToRemove.PositionY);
                var resourceCell = _mineGenerationVariables.GetCell(resourceCellPos);
                if (resourceCell != null) resourceCell.HasResource = false;
                break;
            case "CellPlaceable":
                var cellPlaceables = _mineGenerationVariables.Mine.CellPlaceables;
                var cellPlaceableToRemove = cellPlaceables.FirstOrDefault(placeable => placeable.Id == item.Id);
                if (cellPlaceableToRemove != null)
                    cellPlaceables.Remove(cellPlaceableToRemove);
                else
                {
                    GD.PrintErr("cell placeable is null");
                    return;
                }
                var cellPlaceableCellPos = new Vector2I(cellPlaceableToRemove.PositionX, cellPlaceableToRemove.PositionY);
                var cellPlaceableCell = _mineGenerationVariables.GetCell(cellPlaceableCellPos);
                if (cellPlaceableCell != null) cellPlaceableCell.HasCellPlaceable = false;
                break;
            case "Artifact":
                item.Slot = inventoryManager.GetNextEmptySlot();
                _inventoryDto.Inventory.OccupiedSlots.Add(item.Slot);
                _inventoryDto.Inventory.InventoryItems.Add(item);
                MineActions.OnInventoryUpdate?.Invoke();
                return;
        }
        
        if (_inventoryDto.Inventory.InventoryItems.Any(item1 => item1.Variant == item.Variant))
        {
            item = _inventoryDto.Inventory.InventoryItems.FirstOrDefault(item1 => item1.Variant == item.Variant)!;
            item.Stack++;
        }
        else
        {
            item.Slot = inventoryManager.GetNextEmptySlot();
            _inventoryDto.Inventory.OccupiedSlots.Add(item.Slot);
            _inventoryDto.Inventory.InventoryItems.Add(item);
        }
        MineActions.OnInventoryUpdate?.Invoke();
    }
}