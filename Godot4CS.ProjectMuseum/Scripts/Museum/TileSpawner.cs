using Godot;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class TileSpawner : TileMap
{
	[Export] private PackedScene _wallRight;
	[Export] private PackedScene _wallLeft;
	[Export] private int _dirtyWallProbability = 10;
	[Export] private Array<Texture2D> _dirtyWallTextures;
	[Export] private Node2D _wallsParent;
	// [Export] private Array<int> _dirtyTilesIndex;
	public override void _Ready()
	{
		HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest");
		httpRequest.Request($"{ApiAddress.UrlPrefix}Museum/GetAllMuseumTiles");
	}
	
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		var minCellIndexX = int.MaxValue;
		var minCellIndexY = int.MaxValue;
		
		foreach (var museumTile in museumTiles)
		{
			if (museumTile.XPosition < minCellIndexX) minCellIndexX = museumTile.XPosition;
			if (museumTile.YPosition < minCellIndexY) minCellIndexY = museumTile.YPosition;
			SetCell(0, new Vector2I( museumTile.XPosition, museumTile.YPosition), museumTile.TileSetNumber, Vector2I.Zero);
		}
		foreach (var museumTile in museumTiles)
		{
			if (museumTile.XPosition == minCellIndexX)
			{
				var instance = (Node2D)_wallLeft.Instantiate();
				AddDirtyWallTexture(instance);
				instance.Position = GetCellWorldPosition(museumTile.XPosition,  museumTile.YPosition);
				_wallsParent.AddChild(instance);
			}

			if (museumTile.YPosition == minCellIndexY)
			{
				var instance = (Node2D)_wallRight.Instantiate();
				AddDirtyWallTexture(instance);
				instance.Position = GetCellWorldPosition(museumTile.XPosition, + museumTile.YPosition);
				_wallsParent.AddChild(instance);
			}
		}
		GD.Print($"Min x {minCellIndexX}, Min y {minCellIndexY}");
	}

	private void AddDirtyWallTexture(Node2D instance)
	{
		if (GD.RandRange(1, 100) < _dirtyWallProbability)
		{
			instance.GetNode<Sprite2D>(".").Texture = _dirtyWallTextures[GD.RandRange(0, _dirtyWallTextures.Count - 1)];
		}
	}

	private Vector2 GetCellWorldPosition(int cellX, int cellY)
	{
		// The following line will return the world position of the center of the cell.
		Vector2 cellCenter = MapToLocal(new Vector2I(cellX , cellY ));
		return cellCenter;
	}
}
