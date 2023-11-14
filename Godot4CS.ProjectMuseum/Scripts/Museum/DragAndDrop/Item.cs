using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class Item : Sprite2D
{
    public static Action<float> OnItemPlaced;

    public bool selectedItem = false;
    [Export]
    public string itemType = "small";
    [Export]
    public float ItemPrice = 45.33f;

    [Export] public int numberOfTilesItTakes = 1;
    private List<Vector2I> listOfCoordinateOffsetsToCheck = new List<Vector2I>();
    
    private List<ExhibitPlacementConditionData> _exhibitPlacementConditionDatas;
    private List<ExhibitPlacementConditionData> _listOfMatchingExhibitPlacementConditionDatas;
    private Color _eligibleColor = Colors.Green;
    private Color _ineligibleColor = Colors.Red;

    private Color _originalColor;

    private HttpRequest _httpRequestForExhibitPlacementConditions;
    private HttpRequest _httpRequestForExhibitPlacement;

    public Item()
    {
        _exhibitPlacementConditionDatas = ServiceRegistry.Resolve<List<ExhibitPlacementConditionData>>();
        GD.Print("Item Initialized" );
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // var tileMap = GetTree().Root.GetNode<TileMap>("museum/TileMap");
        // tileMap.AddChild(this);
        // //
        // // // GameManager.TileMap.GetNode()
        // GD.Print("child count " +  tileMap.GetChildCount());
        _httpRequestForExhibitPlacement = new HttpRequest();
        _httpRequestForExhibitPlacementConditions = new HttpRequest();
        AddChild(_httpRequestForExhibitPlacement);
        AddChild(_httpRequestForExhibitPlacementConditions);
        _httpRequestForExhibitPlacementConditions.RequestCompleted += httpRequestForExhibitPlacementConditionsOnRequestCompleted;
        _httpRequestForExhibitPlacement.RequestCompleted += httpRequestForExhibitPlacementOnRequestCompleted;
        string url = ApiAddress.MuseumApiPath + itemType;
        _httpRequestForExhibitPlacementConditions.Request(url);

        _originalColor = Modulate;
        if (numberOfTilesItTakes == 1)
        {
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(0, 0));
        }else if (numberOfTilesItTakes == 4)
        {
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(0, 0));
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(0, -1));
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(-1, 0));
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(-1, -1));
        }else if (numberOfTilesItTakes == 2)
        {
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(0, 0));
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(0, -1));
            
        }
    }

    private void httpRequestForExhibitPlacementConditionsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        _exhibitPlacementConditionDatas = JsonSerializer.Deserialize<List<ExhibitPlacementConditionData>>(jsonStr);
        // GD.Print(jsonStr);
    }
    private void httpRequestForExhibitPlacementOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
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
        Modulate = eligibleForItemPlacementInTile ? _eligibleColor : _ineligibleColor;
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
           
            HandleExhibitPlacement();
            OnItemPlaced?.Invoke(ItemPrice);
            selectedItem = false;
            Modulate = _originalColor;
        }
        if (selectedItem && Input.IsActionPressed("ui_right_click"))
        {
            QueueFree();
        }
    }

    private async void HandleExhibitPlacement()
    {
        foreach (var matchingExhibitPlacementConditionData in _listOfMatchingExhibitPlacementConditionDatas)
        {
            string url =
                $"{ApiAddress.MuseumApiPath}PlaceAnExhibit/{GetTileId(new Vector2I(matchingExhibitPlacementConditionData.TileXPosition, matchingExhibitPlacementConditionData.TileYPosition))}/{itemType}";
            _httpRequestForExhibitPlacement.Request(url);
            await Task.Delay(300);
        }

        
        
    }

    private bool CheckIfTheTileIsEligible(Vector2I tilePosition)
    {
        if (_exhibitPlacementConditionDatas is not { Count: > 0 }) return false;
        _listOfMatchingExhibitPlacementConditionDatas = new List<ExhibitPlacementConditionData>();
        foreach (var offsetCoordinate in listOfCoordinateOffsetsToCheck)
        {
            foreach (var exhibitPlacementConditionData in _exhibitPlacementConditionDatas)
            {
                if (exhibitPlacementConditionData.TileXPosition == tilePosition.X + offsetCoordinate.X &&
                    exhibitPlacementConditionData.TileYPosition == tilePosition.Y + offsetCoordinate.Y)
                {
                    _listOfMatchingExhibitPlacementConditionDatas.Add(exhibitPlacementConditionData);
                }

            }
        }

        if (_listOfMatchingExhibitPlacementConditionDatas.Count < numberOfTilesItTakes) return false;
        
        foreach (var matchingData in _listOfMatchingExhibitPlacementConditionDatas)
        {
            if (!matchingData.IsEligible)
            {
                return false;
            }
            
        }
        GD.Print($"{_listOfMatchingExhibitPlacementConditionDatas.Count} eligible tiles");
        return true;
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