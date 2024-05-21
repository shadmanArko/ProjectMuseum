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
using Node2D = Godot.Node2D;

public partial class SanitationItem : Item
{
	private string _variationName;
	private HttpRequest _httpRequestForPlacingSanitationItem;
	private BuilderCardType _builderCardType;
	public override void _Ready()
	{
		base._Ready();
		_httpRequestForPlacingSanitationItem = new HttpRequest();
		AddChild(_httpRequestForPlacingSanitationItem);
		_httpRequestForPlacingSanitationItem.RequestCompleted += HttpRequestForPlacingSanitationItemOnRequestCompleted;
	}

	private void HttpRequestForPlacingSanitationItemOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		GD.Print("Http1 result " + jsonStr);
		var tilesWithSanitationsDto = JsonSerializer.Deserialize<TilesWithSanitationsDTO>(jsonStr);
		_museumTileContainer.MuseumTiles = tilesWithSanitationsDto.MuseumTiles;
		_museumTileContainer.Sanitations = tilesWithSanitationsDto.Sanitations!;
	}

	public override void _PhysicsProcess(double delta)
	{
		CheckForSeeThroughEffect();
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

	private void CheckForSeeThroughEffect()
	{
		var player = GetTree().Root.GetNode<CharacterBody2DIsometric>("Museum Scene Di Installer/museum/Player");
		if (player!=null)
		{
			if (GetRect().Intersects(player._characterSprite.GetRect()))
			{
				GD.Print("player inside bound");
			}
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
		GD.Print("Handled item placement from Sanitation Item");
		List<string> tileIds = new List<string>();
		foreach (var matchingExhibitPlacementConditionData in _listOfMatchingExhibitPlacementConditionDatas)
		{
			tileIds.Add(GetTileId(new Vector2I(matchingExhibitPlacementConditionData.TileXPosition, matchingExhibitPlacementConditionData.TileYPosition)));
		}
		string[] headers = { "Content-Type: application/json"};
		var body = JsonConvert.SerializeObject(tileIds);
		string url = "";
		GD.Print($"sanitation variation {_variationName}, body {body} ");
		url = $"{ApiAddress.MuseumApiPath}PlaceSanitationOnTiles/{tileIds[0]}/{_variationName}/{Frame}";
		_httpRequestForPlacingSanitationItem.Request(url, headers, HttpClient.Method.Get, body);
		MuseumActions.OnMuseumBalanceReduced?.Invoke(ItemPrice);
		MuseumActions.OnItemUpdated?.Invoke();
		OnItemPlacedOnTile(GlobalPosition);
		DisableItemPlacementShadow();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForPlacingSanitationItem.RequestCompleted -= HttpRequestForPlacingSanitationItemOnRequestCompleted;

	}
}
