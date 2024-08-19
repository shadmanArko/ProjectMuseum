using System.Collections.Generic;
using System.Text;
using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum.Managers;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Service.MuseumServices;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Newtonsoft.Json;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;
namespace Godot4CS.ProjectMuseum.Scripts.Museum.DragAndDrop;

public partial class ExhibitItem : Item
{
	[Export] private Array<Sprite2D> _artifactSlots;
	private HttpRequest _httpRequestForGettingExhibitVariation;
	private ItemPlacementConditionService _itemPlacementConditionService;
	public override void _Ready()
	{
		base._Ready();
		_itemPlacementConditionService = MuseumReferenceManager.Instance.ItemPlacementConditionService;
		_httpRequestForGettingExhibitVariation = new HttpRequest();
		AddChild(_httpRequestForGettingExhibitVariation);
		// _httpRequestForGettingExhibitVariation.RequestCompleted += HttpRequestForGettingExhibitVariationOnRequestCompleted;
		MuseumActions.ArtifactDroppedOnExhibitSlot += ArtifactDroppedOnExhibitSlot;
		MuseumActions.ArtifactRemovedFromExhibitSlot += ArtifactRemovedFromExhibitSlot;
		// _httpRequestForArtifactPlacement.RequestCompleted += HttpRequestForArtifactPlacementOnRequestCompleted;
		// _httpRequestForArtifactRemoval.RequestCompleted += HttpRequestForArtifactRemovalOnRequestCompleted;
		MuseumActions.OnExhibitDeleted += OnExhibitDeleted;
		MuseumActions.OnMakeExhibitFloatForMoving += OnMakeExhibitFloatForMoving;
	}

	private void OnMakeExhibitFloatForMoving(string obj)
	{
		if (obj == ExhibitData.Id)
		{
			selectedItem = true;
			_moving = true;
			_movingFromPos = Position;
			GetUpdatedItemPlacementConditions();
		}

	}

	private void OnExhibitDeleted(string obj)
	{
		if (obj == ExhibitData.Id)
		{
			QueueFree();
		}
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
			// Modulate = _eligibleForItemPlacementInTile ? _eligibleColor : _ineligibleColor;
			SetMaterialBasedOnEligibility();
			// GD.Print( "blend value " +itemShaderMaterial.GetShaderParameter("blend"));
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

			if (!_moving)
			{
				HandleItemPlacement();
			}
			else
			{
				HandleItemMovedPlacement();
			}

			// OnItemPlaced?.Invoke(ItemPrice);
			selectedItem = false;
			OnItemPlacedOnTile(GlobalPosition);
			// Offset = new  Vector2(Offset.X, Offset.Y + _offsetBeforeItemPlacement);
			Modulate = _originalColor;
			SetMaterialWithoutBlend();

		}
		if (selectedItem && Input.IsActionPressed("ui_right_click"))
		{
			if (_moving)
			{
				Position = _movingFromPos;
				selectedItem = false;
				Modulate = _originalColor;
				SetMaterialWithoutBlend();
			}
			else
			{
				QueueFree();
			}

		}
	}

	private void HandleItemMovedPlacement()
	{
		//GD.Print("Handled item placement from ExhibitItem");
		List<string> tileIds = new List<string>();
		foreach (var matchingExhibitPlacementConditionData in _listOfMatchingExhibitPlacementConditionDatas)
		{
			tileIds.Add(GetTileId(new Vector2I(matchingExhibitPlacementConditionData.TileXPosition, matchingExhibitPlacementConditionData.TileYPosition)));

		}

		var exhibitWithNewTiles = new ExhibitWithNewTiles() { Exhibit = ExhibitData, NewTileIds = tileIds };
		string[] headers = { "Content-Type: application/json" };
		var body = JsonConvert.SerializeObject(exhibitWithNewTiles);
		string url =
			$"{ApiAddress.MuseumApiPath}MoveExhibitOnTiles/{tileIds[0]}/{Frame}";
		_httpRequestForExhibitPlacement.Request(url, headers, HttpClient.Method.Get, body);
		//GD.Print($"Handling exhibit placement for price {ItemPrice}");
		// MuseumActions.OnMuseumBalanceReduced?.Invoke(ItemPrice);
		GetUpdatedItemPlacementConditions();
		MuseumActions.OnItemUpdated?.Invoke();
		DisableItemPlacementShadow();
	}

	private new void HandleItemPlacement()
	{
		//GD.Print("Handled item placement from ExhibitItem");
		List<string> tileIds = new List<string>();
		foreach (var matchingExhibitPlacementConditionData in _listOfMatchingExhibitPlacementConditionDatas)
		{
			tileIds.Add(GetTileId(new Vector2I(matchingExhibitPlacementConditionData.TileXPosition, matchingExhibitPlacementConditionData.TileYPosition)));

		}


		string[] headers = { "Content-Type: application/json" };
		var body = JsonConvert.SerializeObject(tileIds);
		string url =
			$"{ApiAddress.MuseumApiPath}PlaceAnExhibitOnTiles/{tileIds[0]}/{ExhibitVariationName}/{Frame}";
		// _httpRequestForExhibitPlacement.Request(url, headers, HttpClient.Method.Get, body);
		var dto = _itemPlacementConditionService.PlaceExhibitOnTiles(tileIds[0], tileIds, ExhibitVariationName, Frame);
		MuseumRunningDataContainer.MuseumTiles = dto.MuseumTiles;
		MuseumRunningDataContainer.Exhibits = dto.Exhibits;
		ExhibitData = dto.Exhibit;
		//GD.Print($"Handling exhibit placement for price {ItemPrice}");
		MuseumActions.OnMuseumBalanceReduced?.Invoke(ItemPrice);
		MuseumActions.OnItemUpdated?.Invoke();
		DisableItemPlacementShadow();
	}
	public void SetUpArtifacts(List<Artifact> displayArtifact)
	{
		foreach (var artifact in displayArtifact)
		{
			if (artifact == null) continue;

			foreach (var gridSlots2X2 in ExhibitData.ArtifactGridSlots2X2s)
			{
				if (artifact.Id == gridSlots2X2.Slot0)
				{
					AssignArtifactToSlot(artifact, 1);
				}
				else if (artifact.Id == gridSlots2X2.Slot1)
				{
					AssignArtifactToSlot(artifact, 2);
				}
				else if (artifact.Id == gridSlots2X2.Slot2)
				{
					AssignArtifactToSlot(artifact, 3);
				}
				else if (artifact.Id == gridSlots2X2.Slot3)
				{
					AssignArtifactToSlot(artifact, 4);
				}
			}
		}
	}


	private void HttpRequestForArtifactRemovalOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		ExhibitData = JsonSerializer.Deserialize<Exhibit>(jsonStr);
		MuseumActions.OnExhibitUpdated?.Invoke(ExhibitData);
		//GD.Print("Removed Artifact");
	}

	private void ArtifactRemovedFromExhibitSlot(Artifact artifact, Item givenItem, int slotNumber, int gridNumber, string artifactSize)
	{
		if (slotNumber == 0) return;
		if (givenItem == this)
		{
			RemoveArtifactFromSlot(slotNumber);
			// _httpRequestForArtifactRemoval.Request(ApiAddress.MuseumApiPath +
												   // $"AddArtifactToStorageFromExhibit/{artifact.Id}/{ExhibitData.Id}/{slotNumber}/{gridNumber}/{artifactSize}");
		   ExhibitData = MuseumReferenceManager.Instance.ExhibitServices.RemoveArtifactFromExhibit(ExhibitData.Id, artifact.Id,
			   slotNumber, gridNumber, artifactSize);
		}
	}

	private void RemoveArtifactFromSlot(int slotNumber)
	{
		// _artifactSlots[slotNumber-1].Texture = null;
		// if (slotNumber == 1)
		//    {
		//        _artifactSlots[0].Texture = null;
		//    }
		//    else if (slotNumber == 2)
		//    {
		//        _artifactSlots[1].Texture = null;
		//    }
	}

	private void HttpRequestForArtifactPlacementOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		GD.Print("exhibit data: " + jsonStr);
		ExhibitData = JsonSerializer.Deserialize<Exhibit>(jsonStr);
		MuseumActions.OnExhibitUpdated?.Invoke(ExhibitData);
		//GD.Print("Placed Artifact");
	}

	private void ArtifactDroppedOnExhibitSlot(Artifact artifact, Item givenItem, int slotNumber, int gridNumber, string artifactSize)
	{
		if (slotNumber == 0) return;

		if (givenItem == this)
		{
			AssignArtifactToSlot(artifact, slotNumber);
			var url = ApiAddress.MuseumApiPath +
					  $"AddArtifactToExhibitSlotFromStore/{artifact.Id}/{ExhibitData.Id}/{slotNumber}/{gridNumber}/{artifactSize}";
			GD.Print($"url: {url}");
			ExhibitData = MuseumReferenceManager.Instance.ExhibitServices.AddArtifactToExhibit(ExhibitData.Id, artifact.Id,
				slotNumber, gridNumber, artifactSize);

		}
	}

	private void AssignArtifactToSlot(Artifact artifact, int slotNumber)
	{
		if (slotNumber == 0)
		{
			return;
		}
		// _artifactSlots[slotNumber-1].Texture = LoadArtifactTexture(artifact.RawArtifactId);
		// if (slotNumber == 1)
		//    {
		//        _artifactSlots[0].Texture = LoadArtifactTexture(artifact.RawArtifactId);
		//    }
		//    else if (slotNumber == 2)
		//    {
		//        _artifactSlots[1].Texture = LoadArtifactTexture(artifact.RawArtifactId);
		//    }
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
		// _httpRequestForArtifactPlacement.RequestCompleted -= HttpRequestForArtifactPlacementOnRequestCompleted;
		// _httpRequestForArtifactRemoval.RequestCompleted -= HttpRequestForArtifactRemovalOnRequestCompleted;
		// _httpRequestForGettingExhibitVariation.RequestCompleted -= HttpRequestForGettingExhibitVariationOnRequestCompleted;
		MuseumActions.OnExhibitDeleted -= OnExhibitDeleted;
		MuseumActions.OnMakeExhibitFloatForMoving -= OnMakeExhibitFloatForMoving;
	}
}