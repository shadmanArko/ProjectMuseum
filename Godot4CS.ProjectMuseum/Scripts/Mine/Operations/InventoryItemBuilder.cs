using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Objects;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class InventoryItemBuilder : Node2D
{
    private MineGenerationVariables _mineGenerationVariables;

    public override void _Ready()
    {
        InitializeDiReference();
    }

    private void InitializeDiReference()
    {
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }
    
    public void BuildCellPlaceableInventoryItem(Vector2 globalCellPos)
    {
        var cellPos = _mineGenerationVariables.MineGenView.LocalToMap(globalCellPos);
        var cellPlaceables = _mineGenerationVariables.Mine.CellPlaceables;
        var cellPlaceable = cellPlaceables.FirstOrDefault(tempPlaceable =>
            tempPlaceable.PositionX == cellPos.X && tempPlaceable.PositionY == cellPos.Y);
        if(cellPlaceable == null) return;
        cellPlaceables.Remove(cellPlaceable);

        var inventoryItem = new InventoryItem
        {
            Id = cellPlaceable.Id,
            Type = "CellPlaceable",
            Category = cellPlaceable.Category,
            Variant = cellPlaceable.Variant,
            IsStackable = true,
            Name = cellPlaceable.Name,
            Stack = 1,
            Slot = 0,
            PngPath = cellPlaceable.PngPath
        };
        
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cellOffset = new Vector2(cellSize, cellSize) / 2;
        var itemDropPosition = cellPos * cellSize + cellOffset;
        
        var itemDropPath = ReferenceStorage.Instance.ItemDropScenePath;
        var cellPlaceableItem =
            SceneInstantiator.InstantiateScene(itemDropPath, _mineGenerationVariables.MineGenView, itemDropPosition) as ItemDrop;
        if (cellPlaceableItem == null)
        {
            GD.PrintErr("Item drop is null");
            return;
        }
		
        GD.Print("instantiated cell placeable item");
        cellPlaceableItem.InventoryItem = inventoryItem;
    }
}