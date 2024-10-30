using Godot;
using System;
using System.Collections.Generic;

// Represents an item's size and position in the grid
public struct ItemGridData
{
    public Vector2I Size;
    public Vector2I Position;
    
    public ItemGridData(Vector2I size, Vector2I position)
    {
        Size = size;
        Position = position;
    }
}

public partial class TestGridSnapping : Control
{
    [Export] private PackedScene _item;
    [Export] private GridContainer GridContainer;
    private Control _currentItem;
    
    // Grid properties
    private const int GRID_ROWS = 4;
    private const int GRID_COLS = 4;
    private const int CELL_SIZE = 100; // Size of each grid cell in pixels
    private bool[,] _occupiedSlots;
    private Dictionary<Control, ItemGridData> _placedItems;
    
    // Visual feedback colors
    private readonly Color VALID_COLOR = new Color(0, 1, 0, 0.5f);    // Green
    private readonly Color INVALID_COLOR = new Color(1, 0, 0, 0.5f);  // Red
    private readonly Color NORMAL_COLOR = new Color(1, 1, 1, 1);      // White

    private bool _isDragging = false;
    
    public override void _Ready()
    {
        _occupiedSlots = new bool[GRID_ROWS, GRID_COLS];
        _placedItems = new Dictionary<Control, ItemGridData>();
        CreateNewDraggingItem();

        // Configure GridContainer
        GridContainer.Columns = GRID_COLS;
        // Add empty cells to visualize the grid (optional)
        // for (int i = 0; i < GRID_ROWS * GRID_COLS; i++)
        // {
        //     var cell = new ColorRect
        //     {
        //         CustomMinimumSize = new Vector2(CELL_SIZE, CELL_SIZE),
        //         Color = new Color(0.2f, 0.2f, 0.2f, 0.5f)
        //     };
        //     GridContainer.AddChild(cell);
        // }
    }

    private void CreateNewDraggingItem()
    {
        _currentItem = _item.Instantiate<Control>();
        AddChild(_currentItem);
        _isDragging = true;
        
        // Set initial size (you can modify this based on item type)
        Vector2I itemSize = new Vector2I(2, 2); // Example: 2x2 item
        _placedItems[_currentItem] = new ItemGridData(itemSize, Vector2I.Zero);

        // Set the size of the item visually
        if (_currentItem is ColorRect colorRect)
        {
            colorRect.CustomMinimumSize = new Vector2(
                itemSize.X * CELL_SIZE,
                itemSize.Y * CELL_SIZE
            );
        }
    }

    public override void _Process(double delta)
    {
        if (_currentItem != null && _isDragging)
        {
            // Update item position to follow mouse
            Vector2 mousePos = GetGlobalMousePosition();
            _currentItem.Position = mousePos;
            
            // Get grid position
            Vector2I gridPos = GetGridPosition(mousePos);
            
            // Check if position is valid and update visual feedback
            bool isValid = IsValidPlacement(gridPos, _placedItems[_currentItem].Size);
            UpdateItemColor(isValid);

            // Handle item placement
            if (Input.IsActionJustPressed("ui_left_click") && isValid)
            {
                PlaceItem(gridPos);
            }
        }
    }

    private Vector2I GetGridPosition(Vector2 globalPos)
    {
        // Get grid container's global position
        Vector2 gridPos = GridContainer.GlobalPosition;
        
        // Calculate relative position to grid
        Vector2 relativePos = globalPos - gridPos;
        
        // Convert to grid coordinates
        return new Vector2I(
            (int)(relativePos.X / CELL_SIZE),
            (int)(relativePos.Y / CELL_SIZE)
        );
    }

    private Vector2 GetGridCellPosition(Vector2I gridPos)
    {
        return GridContainer.GlobalPosition + new Vector2(
            gridPos.X * CELL_SIZE,
            gridPos.Y * CELL_SIZE
        );
    }

    private bool IsValidPlacement(Vector2I gridPos, Vector2I itemSize)
    {
        // Check if item is within grid bounds
        if (gridPos.X < 0 || gridPos.Y < 0 || 
            gridPos.X + itemSize.X > GRID_COLS || 
            gridPos.Y + itemSize.Y > GRID_ROWS)
            return false;

        // Check if all required slots are available
        for (int x = 0; x < itemSize.X; x++)
        {
            for (int y = 0; y < itemSize.Y; y++)
            {
                if (_occupiedSlots[gridPos.Y + y, gridPos.X + x])
                    return false;
            }
        }

        return true;
    }

    private void PlaceItem(Vector2I gridPos)
    {
        ItemGridData itemData = _placedItems[_currentItem];
        
        // Mark slots as occupied
        for (int x = 0; x < itemData.Size.X; x++)
        {
            for (int y = 0; y < itemData.Size.Y; y++)
            {
                _occupiedSlots[gridPos.Y + y, gridPos.X + x] = true;
            }
        }

        // Update item data with new position
        _placedItems[_currentItem] = new ItemGridData(itemData.Size, gridPos);
        
        // Snap item to grid
        Vector2 snapPosition = GetGridCellPosition(gridPos);
        _currentItem.GlobalPosition = snapPosition;
        
        // Move item to grid container
        RemoveChild(_currentItem);
        GridContainer.AddChild(_currentItem);
        _currentItem.Position = new Vector2(
            gridPos.X * CELL_SIZE,
            gridPos.Y * CELL_SIZE
        );
        
        // Reset dragging state and create new item
        _isDragging = false;
        CreateNewDraggingItem();
    }

    private void UpdateItemColor(bool isValid)
    {
        if (_currentItem is ColorRect colorRect)
        {
            colorRect.Color = isValid ? VALID_COLOR : INVALID_COLOR;
        }
    }

    private void RemoveItem(Control item)
    {
        if (_placedItems.TryGetValue(item, out ItemGridData itemData))
        {
            // Clear occupied slots
            for (int x = 0; x < itemData.Size.X; x++)
            {
                for (int y = 0; y < itemData.Size.Y; y++)
                {
                    _occupiedSlots[itemData.Position.Y + y, itemData.Position.X + x] = false;
                }
            }
            
            _placedItems.Remove(item);
            item.QueueFree();
        }
    }
}