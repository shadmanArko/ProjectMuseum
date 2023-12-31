using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class MuseumTileEditor : Node2D
{
    [Export] private int _tileSourceId;
    private TileMap _tileMap;
    private int _tileSize;
    private HttpRequest _httpRequestForGettingTileVariations;
    private HttpRequest _httpRequestForUpdatingTilesSourceId;
    private List<TileVariation> _tileVariations;
    private string _museumTileVariationsApiPath = "";
    private MuseumTileContainer _museumTileContainer;
    private bool _canEditTiles = false;
    public override async void _Ready()
    {
        base._Ready();
        _tileMap = GameManager.TileMap;
        _museumTileVariationsApiPath = ApiAddress.MuseumApiPath + "GetAllTileVariations";
        _httpRequestForGettingTileVariations = new HttpRequest();
        _httpRequestForUpdatingTilesSourceId = new HttpRequest();
        AddChild(_httpRequestForGettingTileVariations);
        AddChild(_httpRequestForUpdatingTilesSourceId);
        _httpRequestForGettingTileVariations.RequestCompleted += OnRequestForGettingTileVariationsCompleted;
        _httpRequestForUpdatingTilesSourceId.RequestCompleted += OnRequestForUpdatingTilesSourceIdCompleted;
        _httpRequestForGettingTileVariations.Request(_museumTileVariationsApiPath);
        MuseumActions.OnClickBuilderCard += OnClickBuilderCard; 
        await Task.Delay(1000);
        _museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
        GD.Print("museumTileEditor is ready");
    }

    private TileVariation _selectedTileVariation;
    private void OnClickBuilderCard(BuilderCardType cardType, string cardName)
    {
        if (cardType == BuilderCardType.Flooring)
        {
            foreach (var tileVariation in _tileVariations)
            {
                if (tileVariation.VariationName == cardName)
                {
                    _selectedTileVariation = tileVariation;
                    _tileSourceId = tileVariation.SourceId;
                    _canEditTiles = true;
                }
            }
        }
        else
        {
            _canEditTiles = false;
        }
    }

    private void OnRequestForUpdatingTilesSourceIdCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        GD.Print("source id put done");
        string jsonStr = Encoding.UTF8.GetString(body);
        _museumTileContainer.MuseumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
    }

    private void OnRequestForGettingTileVariationsCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        _tileVariations = JsonSerializer.Deserialize<List<TileVariation>>(jsonStr);
        GD.Print("number of tile variations" + _tileVariations.Count);
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
        if (!_canEditTiles) return;
        
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
                        GameManager.TileMap.SetCell(0, spawnTilePos, _tileSourceId, Vector2I.Zero);
                        // GameManager.TileMap.ClearLayer(0, new Vector2I(x, y), 0, Vector2I.Zero);
                        
                    }
                }

                UpdateTileSourceIdToDatabase(tilePositions, _tileSourceId);
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
                        GameManager.TileMap.SetCell(1, new Vector2I(x, y), _tileSourceId, Vector2I.Zero);
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
            foreach (var museumTile in _museumTileContainer.MuseumTiles)
            {
                if (museumTile.XPosition == tilePosition.X && museumTile.YPosition == tilePosition.Y && museumTile.TileSetNumber != sourceId)
                {
                    tileIds.Add(museumTile.Id);
                }
            }
        }
        MuseumActions.OnMuseumBalanceReduced?.Invoke(tileIds.Count * _selectedTileVariation.Price );
        string[] headers = { "Content-Type: application/json"};
        var body = JsonConvert.SerializeObject(tileIds);
        Error error = _httpRequestForUpdatingTilesSourceId.Request(ApiAddress.MuseumApiPath+ $"UpdateMuseumTilesWallId?sourceId={sourceId}", headers,
            HttpClient.Method.Put, body);
    }

    public override void _ExitTree()
    {
        _httpRequestForGettingTileVariations.RequestCompleted -= OnRequestForGettingTileVariationsCompleted;
        _httpRequestForUpdatingTilesSourceId.RequestCompleted -= OnRequestForUpdatingTilesSourceIdCompleted;
        MuseumActions.OnClickBuilderCard -= OnClickBuilderCard; 
    }
}