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
    
    public void BuildInventoryItem(string type, Vector2 pos)
    {
        var cellPos = _mineGenerationVariables.MineGenView.LocalToMap(pos);
        var cell = _mineGenerationVariables.GetCell(cellPos);
        if(cell == null) return;
        var inventoryItem = GetInventoryItem(type, cell);
        if (inventoryItem == null)
        {
            GD.PrintErr($"Fatal Error: Inventory item could not be created for {type}");
            return;
        }
        
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cellOffset = new Vector2(cellSize, cellSize) / 2;
        var itemDropPosition = cellPos * cellSize + cellOffset;
        
        var itemDropPath = ReferenceStorage.Instance.ItemDropScenePath;
        var inventoryItemObject =
            SceneInstantiator.InstantiateScene(itemDropPath, _mineGenerationVariables.MineGenView, itemDropPosition) as ItemDrop;
        if (inventoryItemObject == null)
        {
            GD.PrintErr("Item drop is null");
            return;
        }
        
        GD.Print("instantiated cell placeable item");
        inventoryItemObject.InventoryItem = inventoryItem;
    }

    private InventoryItem GetInventoryItem(string type, Cell cell)
    {
        var cellPos = new Vector2I(cell.PositionX, cell.PositionY);
        switch (type)
        {
            case "CellPlaceable":
                var cellPlaceables = _mineGenerationVariables.Mine.CellPlaceables;
                var cellPlaceable = cellPlaceables.FirstOrDefault(tempPlaceable =>
                    tempPlaceable.PositionX == cellPos.X && tempPlaceable.PositionY == cellPos.Y);
                if (cellPlaceable == null)
                {
                    GD.PrintErr($"Fatal Error: Could not find cell placeable");
                    return null;
                }
                cell.HasCellPlaceable = false;
                
                var cellPlaceableItem = new InventoryItem
                {
                    Id = cellPlaceable.Id,
                    Type = type,
                    Category = cellPlaceable.Category,
                    Variant = cellPlaceable.Variant,
                    IsStackable = true,
                    Name = cellPlaceable.Name,
                    Stack = 1,
                    Slot = 0,
                    PngPath = cellPlaceable.PngPath
                };
                return cellPlaceableItem;
            
            case "Resource":
                var resources = _mineGenerationVariables.Mine.Resources;
                var resource = resources.FirstOrDefault(tempResource =>
                    tempResource.PositionX == cellPos.X && tempResource.PositionY == cellPos.Y);
                if (resource == null)
                {
                    GD.PrintErr($"Fatal Error: Could not find resource");
                    return null;
                }
                cell.HasResource = false;
                var resourceItem = new InventoryItem
                {
                    Id = resource.Id,
                    Type = type,
                    Category = resource.Category,
                    Variant = resource.Variant,
                    IsStackable = true,
                    Name = resource.Name,
                    Stack = 1,
                    Slot = 0,
                    PngPath = resource.PNGPath
                };
                return resourceItem;
        }
        
        GD.PrintErr($"Type Mismatch!");
        return null;
    }
}