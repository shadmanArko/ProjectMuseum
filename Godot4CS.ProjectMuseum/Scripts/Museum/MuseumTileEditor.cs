using System;
using System.Diagnostics;
using Godot;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class MuseumTileEditor : Control
{
    private TileMap _tileMap;
    private int _tileSize;
    public override void _Ready()
    {
        base._Ready();
        _tileMap = GameManager.TileMap;
        
        GD.Print("museumTileEditor is ready");
    }

    // public override void _Input(InputEvent @event)
    // {
    //     
    //     if (Input.IsActionJustPressed("ui_left_click"))
    //     {
    //         // Left mouse button was pressed
    //         Vector2 clickPosition = GetGlobalMousePosition();
    //         Vector2I tilePosition  = GameManager.TileMap.LocalToMap(clickPosition);
    //         GameManager.TileMap.SetCell(0, tilePosition, 0, Vector2I.Zero);
    //         GD.Print("Mouse clicked at: " + tilePosition);
    //     }
    //     
    // }
    private Vector2 _dragStartPosition;
    private Vector2I _selectedTile;
    private Rect2 _selectionRect;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
           
            if (Input.IsActionJustPressed("ui_left_click"))
            {
                // Left mouse button pressed
                _tileSize = GameManager.TileMap.CellQuadrantSize;
                _dragStartPosition = GetGlobalMousePosition();
                _selectedTile = GameManager.TileMap.LocalToMap(_dragStartPosition);
            }
            else if (Input.IsActionJustReleased("ui_left_click"))
            {
                // Left mouse button released
                Vector2 dragEndPosition = GetGlobalMousePosition();
                Vector2I endTile = GameManager.TileMap.LocalToMap(dragEndPosition);

                // Iterate over the tiles in the box
                for (int x = Math.Min(_selectedTile.X, endTile.X); x <= Math.Max(_selectedTile.X, endTile.X); x++)
                {
                    for (int y = Math.Min(_selectedTile.Y, endTile.Y);
                         y <= Math.Max(_selectedTile.Y, endTile.Y);
                         y++)
                    {
                        GameManager.TileMap.SetCell(0, new Vector2I(x, y), 0, Vector2I.Zero);
                        
                    }
                }
            }
            
        }else if (@event is InputEventMouseMotion mouseMotion)
        {
            // Update the selection rectangle during mouse motion
            if (Input.IsActionPressed("ui_left_click"))
            {
                Vector2 dragCurrentPosition = GetGlobalMousePosition();
                Vector2I currentTileIso = WorldToIso(dragCurrentPosition);
                Vector2I currentTile = GameManager.TileMap.LocalToMap(dragCurrentPosition);
                Vector2 selectedTileLocalPosition = GameManager.TileMap.MapToLocal(_selectedTile);
                Vector2 selectedTileWorldPosition = IsoToWorld(WorldToIso(_dragStartPosition));
                Vector2 topLeft = new Vector2(selectedTileLocalPosition.X, selectedTileLocalPosition.Y);
                Vector2 size = new Vector2(dragCurrentPosition.X - selectedTileLocalPosition.X, dragCurrentPosition.Y - selectedTileLocalPosition.Y);
                _selectionRect = new Rect2(topLeft, size);
                GD.Print("Drawing call");
                QueueRedraw();
            }
        }
    }
    private Vector2I WorldToIso(Vector2 position)
    {
        // Convert screen coordinates to isometric coordinates
        float x = position.X / _tileSize - position.Y / _tileSize;
        float y = position.X / _tileSize + position.Y / _tileSize;
        return new Vector2I((int)x, (int)y);
    }

    private Vector2 IsoToWorld(Vector2I isoPosition)
    {
        // Convert isometric coordinates to screen coordinates
        float x = (isoPosition.X + isoPosition.Y) * _tileSize * 0.5f;
        float y = (isoPosition.Y - isoPosition.X) * _tileSize* 0.5f;
        return new Vector2(x, y);
    }
    public override void _Draw()
    {
        GD.Print("Printing rect");
        // Draw the selection rectangle
        DrawRect(_selectionRect, Colors.Green);
    }
}