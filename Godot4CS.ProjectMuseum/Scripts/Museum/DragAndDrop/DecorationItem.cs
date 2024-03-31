using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Newtonsoft.Json;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

public partial class DecorationItem : Item
{
	private string _variationName;
	private HttpRequest _httpRequestForPlacingDecorationItem;
	private BuilderCardType _builderCardType;
	public override void _Ready()
	{
		base._Ready();
		_httpRequestForPlacingDecorationItem = new HttpRequest();
		AddChild(_httpRequestForPlacingDecorationItem);
		_httpRequestForPlacingDecorationItem.RequestCompleted += HttpRequestForPlacingDecorationItemOnRequestCompleted;
	}

	private void HttpRequestForPlacingDecorationItemOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		GD.Print("Http1 result " + jsonStr);
		var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		_museumTileContainer.MuseumTiles = museumTiles;
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
			Modulate = _originalColor;
		}
		if (selectedItem && Input.IsActionPressed("ui_right_click"))
		{
			QueueFree();
		}
	}

	public void Initialize(string cardName, BuilderCardType builderCardType)
	{
		selectedItem = true;
		_builderCardType = builderCardType;
		_variationName = cardName;
		MakeObjectsFloating();
	}
	private new void HandleItemPlacement()
	{
		GD.Print("Handled item placement from Decoration Item");
		List<string> tileIds = new List<string>();
		foreach (var matchingExhibitPlacementConditionData in _listOfMatchingExhibitPlacementConditionDatas)
		{
			tileIds.Add(GetTileId(new Vector2I(matchingExhibitPlacementConditionData.TileXPosition, matchingExhibitPlacementConditionData.TileYPosition)));
		}
		string[] headers = { "Content-Type: application/json"};
		var body = JsonConvert.SerializeObject(tileIds);
		string url = "";
		
		if (_builderCardType == BuilderCardType.DecorationShop)
		{
			url = $"{ApiAddress.MuseumApiPath}PlaceAShopOnTiles/{tileIds[0]}/{_variationName}/{Frame}";

		}else if (_builderCardType == BuilderCardType.DecorationOther)
		{
			url = $"{ApiAddress.MuseumApiPath}PlaceOtherDecorationOnTiles/{tileIds[0]}/{_variationName}/{Frame}";
		}
		_httpRequestForPlacingDecorationItem.Request(url, headers, HttpClient.Method.Get, body);
		GD.Print("Handling exhibit placement");
		MuseumActions.OnMuseumBalanceReduced?.Invoke(ItemPrice);
		MuseumActions.OnItemUpdated?.Invoke();
		OnItemPlacedOnTile(GlobalPosition);
		DisableItemPlacementShadow();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForPlacingDecorationItem.RequestCompleted -= HttpRequestForPlacingDecorationItemOnRequestCompleted;

	}
}
