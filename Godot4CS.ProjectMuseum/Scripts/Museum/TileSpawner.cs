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
	[Export] private Vector2I _originOfExpansion = new Vector2I(0, -20);
	private HttpRequest _httpRequestForGettingMuseumTiles;
	private HttpRequest _httpRequestForExpandingMuseumTiles;
	// [Export] private Array<int> _dirtyTilesIndex;
	public override void _Ready()
	{
		_httpRequestForGettingMuseumTiles = new HttpRequest();
		_httpRequestForExpandingMuseumTiles = new HttpRequest();
		AddChild(_httpRequestForGettingMuseumTiles);
		AddChild(_httpRequestForExpandingMuseumTiles);
		_httpRequestForGettingMuseumTiles.RequestCompleted += OnRequestCompletedForGettingMuseumTiles;
		_httpRequestForExpandingMuseumTiles.RequestCompleted += HttpRequestForExpandingMuseumTilesOnRequestCompleted;
		_httpRequestForGettingMuseumTiles.Request($"{ApiAddress.UrlPrefix}Museum/GetAllMuseumTiles");
	}

	private void HttpRequestForExpandingMuseumTilesOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		SpawnTilesAndWalls(museumTiles);
	}

	private void OnRequestCompletedForGettingMuseumTiles(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		SpawnTilesAndWalls(museumTiles);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (Input.IsActionPressed("Equipment1"))
		{
			_httpRequestForGettingMuseumTiles.Request($"{ApiAddress.UrlPrefix}Museum/ExpandMuseum/{_originOfExpansion.X}/{_originOfExpansion.Y}");
		}
	}

	private void SpawnTilesAndWalls(List<MuseumTile> museumTiles)
	{
		var minCellIndexX = int.MaxValue;
		var minCellIndexY = int.MaxValue;

		foreach (Node child in _wallsParent.GetChildren())
		{
			child.QueueFree();
		}
		
		foreach (var museumTile in museumTiles)
		{
			if (museumTile.XPosition < minCellIndexX) minCellIndexX = museumTile.XPosition;
			if (museumTile.YPosition < minCellIndexY) minCellIndexY = museumTile.YPosition;
			SetCell(0, new Vector2I(museumTile.XPosition, museumTile.YPosition), museumTile.TileSetNumber, Vector2I.Zero);
		}

		foreach (var museumTile in museumTiles)
		{
			if (museumTile.XPosition == minCellIndexX)
			{
				var instance = (Node2D)_wallLeft.Instantiate();
				AddDirtyWallTexture(instance);
				instance.Position = GetCellWorldPosition(museumTile.XPosition, museumTile.YPosition);
				_wallsParent.AddChild(instance);
			}

			if (museumTile.YPosition == minCellIndexY)
			{
				var instance = (Node2D)_wallRight.Instantiate();
				AddDirtyWallTexture(instance);
				instance.Position = GetCellWorldPosition(museumTile.XPosition, +museumTile.YPosition);
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
