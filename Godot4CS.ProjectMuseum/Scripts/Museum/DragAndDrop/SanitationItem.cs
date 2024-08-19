using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Managers;
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
	private CharacterBody2DIsometric _player;
	private Sanitation _sanitationData;
	public override void _Ready()
	{
		base._Ready();
		_player = GetTree().Root.GetNode<CharacterBody2DIsometric>("Museum Scene Di Installer/museum/Player");
		_httpRequestForPlacingSanitationItem = new HttpRequest();
		AddChild(_httpRequestForPlacingSanitationItem);
		_httpRequestForPlacingSanitationItem.RequestCompleted += HttpRequestForPlacingSanitationItemOnRequestCompleted;
	}

	private void HttpRequestForPlacingSanitationItemOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		GD.Print("Http1 result " + jsonStr);
		var tilesWithSanitationsDto = JsonSerializer.Deserialize<TilesWithSanitationsDTO>(jsonStr);
		MuseumRunningDataContainer.MuseumTiles = tilesWithSanitationsDto.MuseumTiles;
		MuseumRunningDataContainer.Sanitations = tilesWithSanitationsDto.Sanitations!;
	}

	public override void _PhysicsProcess(double delta)
	{
		// CheckForSeeThroughEffect();
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
			SetMaterialWithoutBlend();
		}
		if (selectedItem && Input.IsActionPressed("ui_right_click"))
		{
			QueueFree();
		}
	}

	private void CheckForSeeThroughEffect()
	{
		
		if (_player!=null)
		{
			var characterRect = new Rect2(_player.Position, _player._characterSprite.GetRect().Size);
			if (GetRect().Intersects(characterRect))
			{
				
				GD.Print($"player rect {characterRect} inside bound {GetRect()}");
			}
			
		}
	}

	public override void _Draw()
	{
		// base._Draw();
		// var playerRect = _player._characterSprite.GetRect();
		// // var playerRect = new Rect2(_player.Position, _player._characterSprite.GetRect().Size);
		// DrawRect(new Rect2(_player.Position, _player._characterSprite.GetRect().Size), Colors.Blue);
		// DrawRect(GetRect(), Colors.Red);
		// // DebugRect();
	}

	private void DebugRect()
	{
		GD.Print($"sanitation rect {GetRect()}");
	}

	public void Initialize(string cardName, BuilderCardType builderCardType)
	{
		selectedItem = true;
		_builderCardType = builderCardType;
		_variationName = cardName;
		_itemType = ItemTypes.Sanitation;
		MakeObjectsFloating();
	}
	public void ReInitializeShop(Sanitation sanitation)
	{
		_sanitationData = sanitation;
		_itemType = ItemTypes.Sanitation;
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
		// _httpRequestForPlacingSanitationItem.Request(url, headers, HttpClient.Method.Get, body);
		var result = MuseumReferenceManager.Instance.ItemPlacementConditionService.PlaceSanitationOnTiles(tileIds[0], tileIds,
			_variationName, Frame);
		MuseumRunningDataContainer.MuseumTiles = result.MuseumTiles;
		MuseumRunningDataContainer.Sanitations = result.Sanitations;
		_sanitationData = result.Sanitation;
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
