using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class MuseumTileEditor : Control
{
    private TileMap _tileMap;
    private int _tileSize;
    private HttpRequest _httpRequestForGettingTiles;
    private HttpRequest _httpRequestForUpdatingTilesSourceId;
    private List<MuseumTile> _museumTiles;
    private string _museumTilesApiPath = "";
    public override void _Ready()
    {
        base._Ready();
        _tileMap = GameManager.TileMap;
        _museumTilesApiPath = ApiAddress.MuseumApiPath + "GetAllMuseumTiles";
        _httpRequestForGettingTiles = new HttpRequest();
        _httpRequestForUpdatingTilesSourceId = new HttpRequest();
        AddChild(_httpRequestForGettingTiles);
        AddChild(_httpRequestForUpdatingTilesSourceId);
        _httpRequestForGettingTiles.RequestCompleted += OnRequestForGettingTilesCompleted;
        _httpRequestForUpdatingTilesSourceId.RequestCompleted += OnRequestForUpdatingTilesSourceIdCompleted;
        GD.Print("museumTileEditor is ready");
    }

    private void OnRequestForUpdatingTilesSourceIdCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        GD.Print("source id put done");
    }

    private void OnRequestForGettingTilesCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        _museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
        GD.Print("number of tiles" + _museumTiles.Count);
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
                _httpRequestForGettingTiles.Request(_museumTilesApiPath);
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
                List<Vector2I> tilePositions = new List<Vector2I>(); 
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
                        tilePositions.Add(spawnTilePos);
                        GameManager.TileMap.SetCell(0, spawnTilePos, 0, Vector2I.Zero);
                        // GameManager.TileMap.ClearLayer(0, new Vector2I(x, y), 0, Vector2I.Zero);
                        
                    }
                }

                UpdateTileSourceIdToDatabase(tilePositions, 0);
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

    private void UpdateTileSourceIdToDatabase(List<Vector2I> tilePositions, int sourceId)
    {
        List<string> tileIds = new List<string>();
        foreach (var tilePosition in tilePositions)
        {
            foreach (var museumTile in _museumTiles)
            {
                if (museumTile.XPosition == tilePosition.X && museumTile.YPosition == tilePosition.Y)
                {
                    tileIds.Add(museumTile.Id);
                }
            }
        }
        
        string[] headers = { "Content-Type: application/json"};
        var body = JsonConvert.SerializeObject(tileIds);
        Error error = _httpRequestForUpdatingTilesSourceId.Request(ApiAddress.MuseumApiPath+ $"UpdateMuseumTilesSourceId?sourceId={sourceId}", headers,
            HttpClient.Method.Put, body);
    }
}