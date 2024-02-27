using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Newtonsoft.Json;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

public partial class Item : Sprite2D, IComparable<Item>
{
    
    public  int CompareTo(Item item2)
    {
        // First, compare Y positions
        int yComparison =  Position.Y.CompareTo(item2.Position.Y);

        // If Y positions are the same, compare X positions
        if (yComparison == 0)
        {
            return Position.X.CompareTo(item2.Position.X);
        }

        return yComparison;
    }
    
    public static Action<float> OnItemPlaced;

    public bool selectedItem = false;
    // [Export]
    // public string itemType = "small";
    [Export]
    public float ItemPrice = 45.33f;

    [Export] public int numberOfTilesItTakes = 1;
    [Export] public string TileExtentsInDirection = "Both";
    [Export] private Sprite2D _glass;
    protected List<Vector2I> listOfCoordinateOffsetsToCheck = new List<Vector2I>();
    
    protected List<ExhibitPlacementConditionData> _exhibitPlacementConditionDatas;
    protected List<ExhibitPlacementConditionData> _listOfMatchingExhibitPlacementConditionDatas;
    protected Color _eligibleColor = Colors.Green;
    protected Color _ineligibleColor = Colors.Red;

    protected Color _originalColor;
    protected HttpRequest _httpRequestForExhibitPlacementConditions;
    protected HttpRequest _httpRequestForExhibitPlacement;
    protected HttpRequest _httpRequestForArtifactPlacement;
    protected HttpRequest _httpRequestForArtifactRemoval;
    public string ExhibitVariationName = "default";
    public Exhibit ExhibitData;
    protected MuseumTileContainer _museumTileContainer;
    protected ItemTypes _itemType;
    protected int _currentFrame;
    protected int _maxFrame = 4;
    public Item()
    {
        _exhibitPlacementConditionDatas = ServiceRegistry.Resolve<List<ExhibitPlacementConditionData>>();
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // var tileMap = GetTree().Root.GetNode<TileMap>("museum/TileMap");
        // tileMap.AddChild(this);
        // //
        // // // GameManager.TileMap.GetNode()
        // GD.Print("child count " +  tileMap.GetChildCount());
        _museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
        
        AddToGroup("ManualSortGroup");
        _httpRequestForExhibitPlacement = new HttpRequest();
        _httpRequestForExhibitPlacementConditions = new HttpRequest();
        _httpRequestForArtifactPlacement = new HttpRequest();
        _httpRequestForArtifactRemoval = new HttpRequest();
        AddChild(_httpRequestForExhibitPlacement);
        AddChild(_httpRequestForArtifactPlacement);
        AddChild(_httpRequestForArtifactRemoval);
        AddChild(_httpRequestForExhibitPlacementConditions);
        _httpRequestForExhibitPlacementConditions.RequestCompleted += httpRequestForExhibitPlacementConditionsOnRequestCompleted;
        _httpRequestForExhibitPlacement.RequestCompleted += httpRequestForExhibitPlacementOnRequestCompleted;
        
        string url = ApiAddress.MuseumApiPath + ExhibitVariationName;
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
        }else if (numberOfTilesItTakes == 2 && TileExtentsInDirection == "Left")
        {
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(0, 0));
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(0, -1));
            
        }else if (numberOfTilesItTakes == 2 && TileExtentsInDirection == "Right")
        {
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(0, 0));
            listOfCoordinateOffsetsToCheck.Add(new Vector2I(-1, 0));
            
        }
    }

    
    
    

    private void httpRequestForExhibitPlacementConditionsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        _exhibitPlacementConditionDatas = JsonSerializer.Deserialize<List<ExhibitPlacementConditionData>>(jsonStr);
        // GD.Print("exhibit placement data"+ jsonStr);
    }
    private void httpRequestForExhibitPlacementOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        // GD.Print("Http1 result " + jsonStr);
        TilesWithExhibitDto tilesWithExhibitDto = JsonSerializer.Deserialize<TilesWithExhibitDto>(jsonStr);
        ExhibitData = tilesWithExhibitDto.Exhibit;
        _museumTileContainer.MuseumTiles = tilesWithExhibitDto.MuseumTiles;
        if (tilesWithExhibitDto.Exhibits != null) _museumTileContainer.Exhibits = tilesWithExhibitDto.Exhibits;
        //GD.Print( $"dto exhibit {ExhibitData.ExhibitVariationName}, has tiles {tilesWithExhibitDto.MuseumTiles.Count}");
    }

    protected Vector2I _lastCheckedTile = new Vector2I();

    protected bool _eligibleForItemPlacementInTile = false;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (!selectedItem) return;
        Vector2I mouseTile = GameManager.tileMap.LocalToMap(GetGlobalMousePosition());
        
        // Check if the tile is eligible for this item placement
        if (_lastCheckedTile != mouseTile)
        {
            Vector2 localPos = GameManager.tileMap.MapToLocal(mouseTile);
            Vector2 worldPos = GameManager.tileMap.ToGlobal(localPos);
            _eligibleForItemPlacementInTile = CheckIfTheTileIsEligible(mouseTile);
            Modulate = _eligibleForItemPlacementInTile ? _eligibleColor : _ineligibleColor;
            // GD.Print($"{eligibleForItemPlacementInTile}");
            // Apply effect based on eligibility
            GlobalPosition = worldPos;
            _lastCheckedTile = mouseTile;
            MuseumActions.OnItemUpdated?.Invoke();
        }
        if (selectedItem && Input.IsActionPressed("ui_left_click"))
        {
            if (!_eligibleForItemPlacementInTile)
            {
                //GD.Print("Not Eligible tile");
                return;
            }

            HandleItemPlacement();
            // OnItemPlaced?.Invoke(ItemPrice);
            selectedItem = false;
            Modulate = _originalColor;
        }
        if (selectedItem && Input.IsActionPressed("ui_right_click"))
        {
            QueueFree();
        }
    }

    public void EnableGlass(bool enableGlass)
    {
        _glass.Visible = enableGlass;
    }
    public async void HandleItemPlacement()
    {
        //GD.Print("Handled item placement from Item");
        List<string> tileIds = new List<string>();
        foreach (var matchingExhibitPlacementConditionData in _listOfMatchingExhibitPlacementConditionDatas)
        {
            tileIds.Add(GetTileId(new Vector2I(matchingExhibitPlacementConditionData.TileXPosition, matchingExhibitPlacementConditionData.TileYPosition)));
        }
        string[] headers = { "Content-Type: application/json"};
        var body = JsonConvert.SerializeObject(tileIds);
        string url =
            $"{ApiAddress.MuseumApiPath}PlaceAnExhibitOnTiles/{tileIds[0]}/{ExhibitVariationName}";
        _httpRequestForExhibitPlacement.Request(url, headers, HttpClient.Method.Get, body);
        //GD.Print("Handling exhibit placement");
        MuseumActions.OnMuseumBalanceReduced?.Invoke(ItemPrice);
        MuseumActions.OnItemUpdated?.Invoke();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustReleased("ui_right_click"))
        {
            if (GetRect().HasPoint(GetLocalMousePosition()))
            {
                MuseumActions.OnClickItem?.Invoke(this, ExhibitData);
                MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("SelectItem");
            }
        }
        if (Input.IsActionJustPressed("PressQ") && selectedItem)
        {
            
            Frame = (_currentFrame+=1) % _maxFrame;
            
        }else if (Input.IsActionJustPressed("PressE") && selectedItem)
        {
            if (_currentFrame == 0) _currentFrame = _maxFrame;
            Frame = (_currentFrame-=1) % _maxFrame;
            
        }
    }

    protected bool CheckIfTheTileIsEligible(Vector2I tilePosition)
    {
        if (_exhibitPlacementConditionDatas is not { Count: > 0 })
        {
            return false;
        }
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
        // GD.Print($"{_listOfMatchingExhibitPlacementConditionDatas.Count} eligible tiles");
        return true;
    }
    protected string GetTileId(Vector2I tilePosition)
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


    public override void _ExitTree()
    {
        
        _httpRequestForExhibitPlacementConditions.RequestCompleted -= httpRequestForExhibitPlacementConditionsOnRequestCompleted;
        _httpRequestForExhibitPlacement.RequestCompleted -= httpRequestForExhibitPlacementOnRequestCompleted;
        
    }
}

public enum ItemTypes
{
    Exhibit,
    Decoration
}