using Godot;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Loading_Bar;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

public partial class TileSpawner : TileMap
{
	[Export] private PackedScene _wallRight;
	[Export] private PackedScene _wallLeft;
	[Export] private PackedScene _wallFrontRight;
	[Export] private PackedScene _wallFrontLeft;
	[Export] private int _dirtyWallProbability = 10;
	[Export] private Array<Texture2D> _dirtyWallTextures;
	[Export] private Node2D _wallsParent;
	[Export] private LoadingBarManager _loadingBarManager;
	private string _basicWallsId = "basic_red_wallpaper";
	private string _frontWallsId = "Front_walls";
	[Export] private Vector2I _originOfExpansion = new Vector2I(0, -20);
	private HttpRequest _httpRequestForGettingMuseumTiles;
	private HttpRequest _httpRequestForExpandingMuseumTiles;
	private HttpRequest _httpRequestForUpdatingMuseumWalls;

	private MuseumTileContainer _museumTileContainer;
	// [Export] private Array<int> _dirtyTilesIndex;
	public override async void  _Ready()
	{
		_loadingBarManager.EmitSignal("IncreaseRegisteredTask");
		_loadingBarManager.EmitSignal("IncreaseRegisteredTask");
		//EmitSignal("IncreaseRegisteredTask");
		GD.Print("GG");
		_httpRequestForGettingMuseumTiles = new HttpRequest();
		_httpRequestForExpandingMuseumTiles = new HttpRequest();
		_httpRequestForUpdatingMuseumWalls = new HttpRequest();
		AddChild(_httpRequestForGettingMuseumTiles);
		AddChild(_httpRequestForExpandingMuseumTiles);
		AddChild(_httpRequestForUpdatingMuseumWalls);
		_httpRequestForGettingMuseumTiles.RequestCompleted += OnRequestCompletedForGettingMuseumTiles;
		_httpRequestForExpandingMuseumTiles.RequestCompleted += HttpRequestForExpandingMuseumTilesOnRequestCompleted;
		_httpRequestForUpdatingMuseumWalls.RequestCompleted += HttpRequestForUpdatingMuseumWallsOnRequestCompleted;
		await Task.Delay(1000);
		_httpRequestForGettingMuseumTiles.Request($"{ApiAddress.UrlPrefix}Museum/GetAllMuseumTiles");
	}

	private void HttpRequestForUpdatingMuseumWallsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		
		string jsonStr = Encoding.UTF8.GetString(body);
		// GD.Print($"wall id put done {jsonStr}");

		var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		_museumTileContainer.MuseumTiles = museumTiles;
		SpawnWalls(museumTiles);
		
		//EmitSignal(LoadingBarManager.SignalName.IncreaseCompletedTask);
	}

	private void SpawnWalls(List<MuseumTile> museumTiles)
	{
		foreach (var museumTile in museumTiles)
		{
			if (museumTile.BackLeftWallId.Length> 0)
			{
				InstantiateWall(museumTile, _wallLeft, WallSide.BackLeft);
			}

			if (museumTile.BackRightWallId.Length> 0)
			{
				InstantiateWall(museumTile, _wallRight, WallSide.BackRight);
			}
			if (museumTile.FrontLeftWallId.Length> 0)
			{
				InstantiateFrontWall(museumTile, _wallFrontLeft, WallSide.FrontLeft);
			}
			if (museumTile.FrontRightWallId.Length> 0)
			{
				InstantiateFrontWall(museumTile, _wallFrontRight, WallSide.FrontRight);
			}
		}
		_loadingBarManager.EmitSignal("IncreaseCompletedTask");
		GD.Print("museum walls request complete");
		MuseumActions.OnMuseumTilesUpdated?.Invoke();
	}

	private void HttpRequestForExpandingMuseumTilesOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		SpawnTilesAndWalls(museumTiles);

	}

	private void OnRequestCompletedForGettingMuseumTiles(long result, long responseCode, string[] headers, byte[] body)
	{
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
		string jsonStr = Encoding.UTF8.GetString(body);
		var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		SpawnTilesAndWalls(museumTiles);
		_loadingBarManager.EmitSignal("IncreaseCompletedTask");
		_museumTileContainer.MuseumTiles = museumTiles;
		GD.Print("museum tiles request complete");
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (Input.IsActionPressed("Expand"))
		{
			_httpRequestForGettingMuseumTiles.Request($"{ApiAddress.UrlPrefix}Museum/ExpandMuseum/{_originOfExpansion.X}/{_originOfExpansion.Y}");
		}
	}

	private int _minCellIndexX;
	private int _minCellIndexY;
	private void SpawnTilesAndWalls(List<MuseumTile> museumTiles)
	{
		_minCellIndexX = int.MaxValue;
		_minCellIndexY = int.MaxValue;

		foreach (Node child in _wallsParent.GetChildren())
		{
			child.QueueFree();
		}
		
		foreach (var museumTile in museumTiles)
		{
			if (museumTile.XPosition < _minCellIndexX) _minCellIndexX = museumTile.XPosition;
			if (museumTile.YPosition < _minCellIndexY) _minCellIndexY = museumTile.YPosition;
			SetCell(0, new Vector2I(museumTile.XPosition, museumTile.YPosition), museumTile.TileSetNumber, Vector2I.Zero);
			if (museumTile.IsDirty)
			{
				SetCell(2, new Vector2I(museumTile.XPosition, museumTile.YPosition), 14, Vector2I.Zero);
			}
		}

		List<TileWallInfo> museumTilesToUpdateWalls = new List<TileWallInfo>();
		foreach (var museumTile in museumTiles)
		{

			if (museumTiles.Find(tile => tile.XPosition == museumTile.XPosition -1 && tile.YPosition == museumTile.YPosition) == null)
			{
				GD.Print($"Found null back left wall {museumTile.XPosition}, {museumTile.YPosition}");
				TileWallInfo tileWallInfo = new TileWallInfo(museumTile.Id, _basicWallsId, "", "", "");
				museumTilesToUpdateWalls.Add(tileWallInfo);

				// if (museumTile.BackLeftWallId.Length <2)
				// {
				// }
			}
			if (museumTiles.Find(tile => tile.XPosition == museumTile.XPosition && tile.YPosition == museumTile.YPosition -1) == null)
			{
				GD.Print($"Found null back right wall {museumTile.XPosition}, {museumTile.YPosition}");
				TileWallInfo tileWallInfo = new TileWallInfo(museumTile.Id, "", _basicWallsId, "", "");
				museumTilesToUpdateWalls.Add(tileWallInfo);
				// if (museumTile.BackRightWallId.Length <2)
				// {
				//
				// }
			}
			if (museumTiles.Find(tile => tile.XPosition == museumTile.XPosition && tile.YPosition == museumTile.YPosition +1) == null)
			{
				GD.Print($"Found null front left wall {museumTile.XPosition}, {museumTile.YPosition}");
				TileWallInfo tileWallInfo = new TileWallInfo(museumTile.Id, "", "", _frontWallsId, "");
				museumTilesToUpdateWalls.Add(tileWallInfo);
				// if (museumTile.FrontLeftWallId.Length <2)
				// {
				//
				// }
			} 
			if (museumTiles.Find(tile => tile.XPosition == museumTile.XPosition +1 && tile.YPosition == museumTile.YPosition ) == null)
			{
				GD.Print($"Found null front right wall {museumTile.XPosition}, {museumTile.YPosition}");
				TileWallInfo tileWallInfo = new TileWallInfo(museumTile.Id, "", "", "", _frontWallsId);
				museumTilesToUpdateWalls.Add(tileWallInfo);
				// if (museumTile.FrontRightWallId.Length <2)
				// {
				//
				// }
			} 
			
			
		}

		if (museumTilesToUpdateWalls.Count > 0)
		{
			UpdateMuseumTilesToDatabase(museumTilesToUpdateWalls);

		}else SpawnWalls(museumTiles);
		//GD.Print($"Min x {_minCellIndexX}, Min y {_minCellIndexY}");
	}

	private void UpdateMuseumTilesToDatabase(List<TileWallInfo> museumTilesToUpdateWalls)
	{
		if (museumTilesToUpdateWalls.Count > 0)
		{
			// List<MuseumTile> tiles = new List<MuseumTile>();
			// foreach (var museumTile in museumTilesToUpdateWalls)
			// {
			// 	tiles.Add(museumTile);
			// 	
			// }
			// MuseumActions.OnMuseumBalanceReduced?.Invoke(tileIds.Count * _selectedTileVariation.Price );
			string[] headers = { "Content-Type: application/json"};
			var body = JsonConvert.SerializeObject(museumTilesToUpdateWalls);
			//GD.Print(body);
			Error error = _httpRequestForUpdatingMuseumWalls.Request(ApiAddress.MuseumApiPath+ $"UpdateMuseumTilesWallId", headers,
				HttpClient.Method.Post, body);
		}
	}

	private void InstantiateWall(MuseumTile museumTile, PackedScene wall, WallSide wallSide)
	{
		
		var instance = (Node2D)wall.Instantiate();
		instance.GetNode<Wall>(".").SetUpWall(museumTile, wallSide);
		AddDirtyWallTexture(instance);
		instance.Position = GetCellWorldPosition(museumTile.XPosition, museumTile.YPosition);
		_wallsParent.AddChild(instance);
	}
	private void InstantiateFrontWall(MuseumTile museumTile, PackedScene wall, WallSide wallSide)
	{
		if ((wallSide == WallSide.FrontLeft || wallSide == WallSide.FrontRight) 
		    && ((museumTile.XPosition == 0 && museumTile.YPosition >= -19)
		        || (museumTile.XPosition >= -17 && museumTile.YPosition ==0)))
		{
			GD.Print($"Skipped wall {museumTile.XPosition}, {museumTile.YPosition}");
			return;
		}
		var instance = (Node2D)wall.Instantiate();
		// instance.GetNode<Wall>(".").SetUpWall(museumTile, wallSide);
		// AddDirtyWallTexture(instance);
		instance.Position = GetCellWorldPosition(museumTile.XPosition, museumTile.YPosition);
		_wallsParent.AddChild(instance);
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
