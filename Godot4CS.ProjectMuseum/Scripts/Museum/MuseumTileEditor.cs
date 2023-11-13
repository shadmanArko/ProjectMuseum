using System;
using System.Diagnostics;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
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
                        GameManager.TileMap.ClearLayer(1);
                        var spawnTilePos = new Vector2I(x, y);
                        if (!spawnTilePos.IsTilePositionInsideTileMap()) continue;
                        GameManager.TileMap.SetCell(0, spawnTilePos, 0, Vector2I.Zero);
                        // GameManager.TileMap.ClearLayer(0, new Vector2I(x, y), 0, Vector2I.Zero);
                        
                    }
                }
            }
            
        }else if (@event is InputEventMouseMotion mouseMotion)
        {
            // Update the selection rectangle during mouse motion
            GameManager.TileMap.ClearLayer(1);
            Vector2I endTile = GameManager.TileMap.LocalToMap(GetGlobalMousePosition());
            if (Input.IsActionPressed("ui_left_click"))
            {
                for (int x = Math.Min(_selectedTile.X, endTile.X); x <= Math.Max(_selectedTile.X, endTile.X); x++)
                {
                    for (int y = Math.Min(_selectedTile.Y, endTile.Y);
                         y <= Math.Max(_selectedTile.Y, endTile.Y);
                         y++)
                    {
                        GameManager.TileMap.SetCell(1, new Vector2I(x, y), 0, Vector2I.Zero);
                        // GameManager.TileMap.ClearLayer(0, new Vector2I(x, y), 0, Vector2I.Zero);
                        
                    }
                }
            }
        }
    }
    
}