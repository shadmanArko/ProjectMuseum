using System.Collections.Generic;
using System.Text;
using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;
namespace Godot4CS.ProjectMuseum.Scripts.Museum.DragAndDrop;

public partial class ExhibitItem : Item
{
	[Export] private Array<Sprite2D> _artifactSlots;
	private HttpRequest _httpRequestForGettingExhibitVariation;
	public override void _Ready()
	{
		base._Ready();
		_httpRequestForGettingExhibitVariation = new HttpRequest();
		AddChild(_httpRequestForGettingExhibitVariation);
		_httpRequestForGettingExhibitVariation.RequestCompleted += HttpRequestForGettingExhibitVariationOnRequestCompleted;
		MuseumActions.ArtifactDroppedOnExhibitSlot += ArtifactDroppedOnExhibitSlot;
		MuseumActions.ArtifactRemovedFromExhibitSlot += ArtifactRemovedFromExhibitSlot;
		_httpRequestForArtifactPlacement.RequestCompleted += HttpRequestForArtifactPlacementOnRequestCompleted;
		_httpRequestForArtifactRemoval.RequestCompleted += HttpRequestForArtifactRemovalOnRequestCompleted;
		
	}

	private void HttpRequestForGettingExhibitVariationOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var exhibitVariation = JsonSerializer.Deserialize<ExhibitVariation>(jsonStr);
		ItemPrice = exhibitVariation.Price;
		//GD.Print("Placed Artifact");
	}

	public void Initialize(string exhibitVariationName)
	{
        
		ExhibitVariationName = exhibitVariationName;
		selectedItem = true;
		_itemType = ItemTypes.Exhibit;
		_httpRequestForGettingExhibitVariation.Request(ApiAddress.MuseumApiPath +
		                                               $"GetExhibitVariation/variationName?variationName={exhibitVariationName}");
		MakeObjectsFloating();
		//GD.Print("Item Initialized");
	}
	public void SpawnFromDatabase(Exhibit exhibit, List<Artifact> displayArtifacts)
	{
		ExhibitData = exhibit;
		SetUpArtifacts(displayArtifacts);
		Frame = exhibit.RotationFrame;
		_itemType = ItemTypes.Exhibit;
	}
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
			OnItemPlacedOnTile(GlobalPosition);
			// Offset = new  Vector2(Offset.X, Offset.Y + _offsetBeforeItemPlacement);
			Modulate = _originalColor;
		}
		if (selectedItem && Input.IsActionPressed("ui_right_click"))
		{
			QueueFree();
		}
	}
	private new void HandleItemPlacement()
	{
		//GD.Print("Handled item placement from ExhibitItem");
		List<string> tileIds = new List<string>();
		foreach (var matchingExhibitPlacementConditionData in _listOfMatchingExhibitPlacementConditionDatas)
		{
			tileIds.Add(GetTileId(new Vector2I(matchingExhibitPlacementConditionData.TileXPosition, matchingExhibitPlacementConditionData.TileYPosition)));
		}
		string[] headers = { "Content-Type: application/json"};
		var body = JsonConvert.SerializeObject(tileIds);
		string url =
			$"{ApiAddress.MuseumApiPath}PlaceAnExhibitOnTiles/{tileIds[0]}/{ExhibitVariationName}/{Frame}";
		_httpRequestForExhibitPlacement.Request(url, headers, HttpClient.Method.Get, body);
		//GD.Print($"Handling exhibit placement for price {ItemPrice}");
		MuseumActions.OnMuseumBalanceReduced?.Invoke(ItemPrice);
		MuseumActions.OnItemUpdated?.Invoke();
		DisableItemPlacementShadow();
	}
	public void SetUpArtifacts(List<Artifact> displayArtifact)
    {
        foreach (var artifact in displayArtifact)
        {
            if (artifact == null ) continue;
            
            if (artifact.Id == ExhibitData.ExhibitArtifactSlot1)
            {
                AssignArtifactToSlot(artifact, 1);
            }else if (artifact.Id == ExhibitData.ExhibitArtifactSlot2)
            {
                AssignArtifactToSlot(artifact, 2);
            }
        }
    }
    

    private void HttpRequestForArtifactRemovalOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        ExhibitData = JsonSerializer.Deserialize<Exhibit>(jsonStr);
        //GD.Print("Removed Artifact");
    }

    private void ArtifactRemovedFromExhibitSlot(Artifact artifact, Item givenItem, int slotNumber)
    {
        if (slotNumber == 0) return;
        if (givenItem == this)
        {
            RemoveArtifactFromSlot(slotNumber);
            _httpRequestForArtifactRemoval.Request(ApiAddress.MuseumApiPath +
                                                   $"AddArtifactToStorageFromExhibit/{artifact.Id}/{ExhibitData.Id}/{slotNumber}");
        }
    }

    private void RemoveArtifactFromSlot(int slotNumber)
    {
        if (slotNumber == 1)
        {
            _artifactSlots[0].Texture = null;
        }
        else if (slotNumber == 2)
        {
            _artifactSlots[1].Texture = null;
        }
    }

    private void HttpRequestForArtifactPlacementOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        ExhibitData = JsonSerializer.Deserialize<Exhibit>(jsonStr);
        //GD.Print("Placed Artifact");
    }

    private void ArtifactDroppedOnExhibitSlot(Artifact artifact, Item givenItem, int slotNumber)
    {
        if (slotNumber == 0) return;
        
        if (givenItem == this)
        {
            AssignArtifactToSlot(artifact, slotNumber);

            _httpRequestForArtifactPlacement.Request(ApiAddress.MuseumApiPath +
                                                     $"AddArtifactToExhibitSlotFromStore/{artifact.Id}/{ExhibitData.Id}/{slotNumber}");
        }
    }

    private void AssignArtifactToSlot(Artifact artifact, int slotNumber)
    {
        if (slotNumber == 1)
        {
            _artifactSlots[0].Texture = LoadArtifactTexture(artifact.RawArtifactId);
        }
        else if (slotNumber == 2)
        {
            _artifactSlots[1].Texture = LoadArtifactTexture(artifact.RawArtifactId);
        }
    }


    private Texture2D LoadArtifactTexture(string artifactIconName)
    {
        string spritePath = $"res://Assets/2D/Sprites/Isometric View Artifacts/{artifactIconName}.png"; // Change the extension if your sprites have a different format

        // Use ResourceLoader.Load to load the texture
        Texture2D texture = (Texture2D)ResourceLoader.Load(spritePath);

        if (texture == null)
        {
            //GD.Print($"Failed to load texture for artifact: {artifactIconName}");
        }

        return texture;
    }

    public override void _ExitTree()
    {
	    base._ExitTree();
	    MuseumActions.ArtifactDroppedOnExhibitSlot -= ArtifactDroppedOnExhibitSlot;
	    MuseumActions.ArtifactRemovedFromExhibitSlot -= ArtifactRemovedFromExhibitSlot;
	    _httpRequestForArtifactPlacement.RequestCompleted -= HttpRequestForArtifactPlacementOnRequestCompleted;
	    _httpRequestForArtifactRemoval.RequestCompleted -= HttpRequestForArtifactRemovalOnRequestCompleted;
	    _httpRequestForGettingExhibitVariation.RequestCompleted -= HttpRequestForGettingExhibitVariationOnRequestCompleted;
    }
}