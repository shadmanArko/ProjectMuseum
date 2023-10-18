using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class Item : Sprite2D
{
    public bool selectedItem = false;
    [Export]
    public string itemType = "small";

    private List<ExhibitPlacementConditionData> _exhibitPlacementConditionDatas;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        HttpRequest http = GetNode<HttpRequest>("HTTPRequest");
        string url = "http://localhost:5178/api/MuseumTile/" + itemType;
        http.Request(url);
    }

    private void OnHttpRequestRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        _exhibitPlacementConditionDatas = JsonSerializer.Deserialize<List<ExhibitPlacementConditionData>>(jsonStr);
        // GD.Print(jsonStr);
    }
    private void OnHttp1RequestRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        GD.Print("Http1 result " + jsonStr);
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (!selectedItem) return;
        Vector2I mouseTile = GameManager.TileMap.LocalToMap(GetGlobalMousePosition());

        Vector2 localPos = GameManager.TileMap.MapToLocal(mouseTile);
        Vector2 worldPos = GameManager.TileMap.ToGlobal(localPos);

        // Check if the tile is eligible for this item placement
        var eligibleForItemPlacementInTile = CheckIfTheTileIsEligible(mouseTile);
        // GD.Print($"{eligibleForItemPlacementInTile}");
        // Apply effect based on eligibility

        GlobalPosition = worldPos;
        if (selectedItem && Input.IsActionPressed("ui_left_click"))
        {
            if (!eligibleForItemPlacementInTile)
            {
                GD.Print("Not Eligible tile");
                return;
            }
            HttpRequest http1 = GetNode<HttpRequest>("HTTPRequest");
            http1.RequestCompleted += OnHttp1RequestRequestCompleted;
            string url = $"http://localhost:5178/api/MuseumTile/PlaceAnExhibit/{GetTileId(mouseTile)}/{itemType}";
            http1.Request(url);
            // http1.RequestCompleted -= OnHttp1RequestRequestCompleted;
            selectedItem = false;
        }
        if (selectedItem && Input.IsActionPressed("ui_right_click"))
        {
            QueueFree();
        }
    }

    private bool CheckIfTheTileIsEligible(Vector2I tilePosition)
    {
        if (_exhibitPlacementConditionDatas is not { Count: > 0 }) return false;
        
        foreach (var exhibitPlacementConditionData in _exhibitPlacementConditionDatas)
        {
            if (exhibitPlacementConditionData.TileXPosition == tilePosition.X &&
                exhibitPlacementConditionData.TileYPosition == tilePosition.Y)
                return exhibitPlacementConditionData.IsEligible;

        }

        return false;
    }
    private string GetTileId(Vector2I tilePosition)
    {
        if (_exhibitPlacementConditionDatas is not { Count: > 0 }) return null;
        
        foreach (var exhibitPlacementConditionData in _exhibitPlacementConditionDatas)
        {
            if (exhibitPlacementConditionData.TileXPosition == tilePosition.X &&
                exhibitPlacementConditionData.TileYPosition == tilePosition.Y)
                return exhibitPlacementConditionData.Id;

        }

        return null;
    }

   
}