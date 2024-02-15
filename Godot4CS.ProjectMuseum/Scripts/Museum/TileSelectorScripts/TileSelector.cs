using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class TileSelector : Node2D
{
    private TileMap _tileMap;
    private int _tileSize;
    private MuseumTileContainer _museumTileContainer;
    private bool _canEditTiles = true;

    private int _tileSourceId = 13;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
        _tileMap = GameManager.TileMap;
        await Task.Delay(1000);
        _museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    private Vector2 _dragStartPosition;
    private Vector2I _selectedTile;
    private Rect2 _selectionRect;
    public override void _Input(InputEvent @event)
    {
        if (!_canEditTiles) return;
        
        if (@event is InputEventMouseButton mouseEvent)
        {
           
            if (Input.IsActionJustPressed("ui_left_click"))
            {
                // Left mouse button pressed
                _tileSize = _tileMap.CellQuadrantSize;
                _dragStartPosition = GetGlobalMousePosition();
                _selectedTile = _tileMap.LocalToMap(_dragStartPosition);
            }
            else if (Input.IsActionJustReleased("ui_left_click"))
            {
                // Left mouse button released
                Vector2 dragEndPosition = GetGlobalMousePosition();
                Vector2I endTile = _tileMap.LocalToMap(dragEndPosition);
                List<Vector2I> tilePositions = new List<Vector2I>(); 
                // Iterate over the tiles in the box
                for (int x = Math.Min(_selectedTile.X, endTile.X); x <= Math.Max(_selectedTile.X, endTile.X); x++)
                {
                    for (int y = Math.Min(_selectedTile.Y, endTile.Y);
                         y <= Math.Max(_selectedTile.Y, endTile.Y);
                         y++)
                    {
                        // _tileMap.ClearLayer(1);
                        var spawnTilePos = new Vector2I(x, y);
                        if (!spawnTilePos.IsTilePositionInsideTileMap()) continue;
                        tilePositions.Add(spawnTilePos);
                        
                        // _tileMap.SetCell(1, spawnTilePos, _tileSourceId, Vector2I.Zero);
                        // _tileMap.ClearLayer(0, new Vector2I(x, y), 0, Vector2I.Zero);
                        
                    }
                }

                OnSelectionComplete(tilePositions);

            }
            
        }else if (Input.IsActionPressed("ui_left_click") && @event is InputEventMouseMotion mouseMotion)
        {
            // Update the selection rectangle during mouse motion
            _tileMap.ClearLayer(1);
            GD.Print("Clearing Selection");
            Vector2I endTile = _tileMap.LocalToMap(GetGlobalMousePosition());
            if (Input.IsActionPressed("ui_left_click"))
            {
                for (int x = Math.Min(_selectedTile.X, endTile.X); x <= Math.Max(_selectedTile.X, endTile.X); x++)
                {
                    for (int y = Math.Min(_selectedTile.Y, endTile.Y);
                         y <= Math.Max(_selectedTile.Y, endTile.Y);
                         y++)
                    {
                        _tileMap.SetCell(1, new Vector2I(x, y), _tileSourceId, Vector2I.Zero);
                        // _tileMap.ClearLayer(0, new Vector2I(x, y), 0, Vector2I.Zero);
                        
                    }
                }
            }
        }
    }
    private void OnSelectionComplete(List<Vector2I> tilePositions)
    {            
        GD.Print("final Selection");

        List<string> tileIds = new List<string>();
        foreach (var tilePosition in tilePositions)
        {
            _tileMap.SetCell(1, tilePosition, _tileSourceId, Vector2I.Zero);
            foreach (var museumTile in _museumTileContainer.MuseumTiles)
            {
                if (museumTile.XPosition == tilePosition.X && museumTile.YPosition == tilePosition.Y)
                {
                    tileIds.Add(museumTile.Id);
                }
            }
        }

        if (tileIds.Count >= 4)
        {
            MuseumActions.OnSelectTilesForZone?.Invoke(tileIds);
        }
    }
    private void OnZoneCreationUiClosed()
    {
        _tileMap.ClearLayer(1);
    }
    public override void _EnterTree()
    {
        base._EnterTree();
        MuseumActions.OnZoneCreationUiClosed += OnZoneCreationUiClosed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MuseumActions.OnZoneCreationUiClosed -= OnZoneCreationUiClosed;

    }

   
}
