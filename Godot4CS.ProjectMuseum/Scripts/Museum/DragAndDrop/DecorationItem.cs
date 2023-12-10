using Godot;
using System;
using System.Collections.Generic;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Newtonsoft.Json;

public partial class DecorationItem : Item
{
	public override void _PhysicsProcess(double delta)
	{
		if (!selectedItem) return;
		Vector2I mouseTile = GameManager.TileMap.LocalToMap(GetGlobalMousePosition());
        
		// Check if the tile is eligible for this item placement
		if (_lastCheckedTile != mouseTile)
		{
			Vector2 localPos = GameManager.TileMap.MapToLocal(mouseTile);
			Vector2 worldPos = GameManager.TileMap.ToGlobal(localPos);
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
				GD.Print("Not Eligible tile");
				return;
			}

			HandleItemPlacement();
			OnItemPlaced?.Invoke(ItemPrice);
			selectedItem = false;
			Modulate = _originalColor;
		}
		if (selectedItem && Input.IsActionPressed("ui_right_click"))
		{
			QueueFree();
		}
	}

	public void Initialize(string cardName)
	{
		selectedItem = true;
	}
	private new void HandleItemPlacement()
	{
		GD.Print("Handled item placement from Decoration Item");
		// List<string> tileIds = new List<string>();
		// foreach (var matchingExhibitPlacementConditionData in _listOfMatchingExhibitPlacementConditionDatas)
		// {
		// 	tileIds.Add(GetTileId(new Vector2I(matchingExhibitPlacementConditionData.TileXPosition, matchingExhibitPlacementConditionData.TileYPosition)));
		// }
		// string[] headers = { "Content-Type: application/json"};
		// var body = JsonConvert.SerializeObject(tileIds);
		// string url =
		// 	$"{ApiAddress.MuseumApiPath}PlaceAnExhibitOnTiles/{tileIds[0]}/{ExhibitVariationName}";
		// _httpRequestForExhibitPlacement.Request(url, headers, HttpClient.Method.Get, body);
		// GD.Print("Handling exhibit placement");
		MuseumActions.OnItemUpdated?.Invoke();
	}
}
