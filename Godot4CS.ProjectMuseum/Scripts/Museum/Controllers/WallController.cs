using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

public partial class WallController : Node2D
{
	private HttpRequest _httpRequestForUpdatingWalls;

	private MuseumTileContainer _museumTileContainer;

	private List<string> _wallTileIds = new List<string>();

	private bool _listenForWallPaperUpdates = false;

	private string _currentCardName;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		await Task.Delay(500);
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
		_httpRequestForUpdatingWalls = new HttpRequest();
		AddChild(_httpRequestForUpdatingWalls);
		_httpRequestForUpdatingWalls.RequestCompleted += HttpRequestForUpdatingWallsOnRequestCompleted;
		MuseumActions.OnClickBuilderCard += OnClickBuilderCard;
		MuseumActions.OnClickWallForUpdatingWallPaper += OnClickWallForUpdatingWallPaper;
	}

	private void OnClickWallForUpdatingWallPaper(string wallTileId)
	{
		if (!_wallTileIds.Contains(wallTileId))
		{
			_wallTileIds.Add(wallTileId);
		}
	}

	private void HttpRequestForUpdatingWallsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		GD.Print("wall id put done");
		
		string jsonStr = Encoding.UTF8.GetString(body);

		var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		if (museumTiles.Count > 0)
		{
			_museumTileContainer.MuseumTiles = museumTiles;
			MuseumActions.OnWallpaperSuccessfullyUpdated?.Invoke();
		}
	}

	private void OnClickBuilderCard(BuilderCardType builderCardType, string cardName)
	{
		if (builderCardType == BuilderCardType.Wallpaper)
		{
			_wallTileIds.Clear();
			_listenForWallPaperUpdates = true;
			_currentCardName = cardName;
			Texture2D texture2D = GD.Load<Texture2D>($"res://Assets/2D/Sprites/{builderCardType}s/{cardName}.png");
			MuseumActions.OnPreviewWallpaperUpdated?.Invoke(texture2D);
		}
		else
		{
			_wallTileIds.Clear();
			_listenForWallPaperUpdates = false;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased("ui_left_click"))
		{
			CheckForWallPaperUpdate();
		}
	}

	private void CheckForWallPaperUpdate()
	{
		if (_listenForWallPaperUpdates && _wallTileIds.Count > 0)
		{
			var validWallTileIds = new List<string>();
			foreach (var wallTileId in _wallTileIds)
			{
				foreach (var museumTile in _museumTileContainer.MuseumTiles)
				{
					if (museumTile.Id == wallTileId && museumTile.WallId != _currentCardName)
					{
						validWallTileIds.Add(wallTileId);
					}
				}
			}

			if (validWallTileIds.Count > 0)
			{
				MuseumActions.OnMuseumBalanceReduced?.Invoke(validWallTileIds.Count);
				string[] headers = { "Content-Type: application/json"};
				var body = JsonConvert.SerializeObject(validWallTileIds);
				GD.Print(body);
				Error error = _httpRequestForUpdatingWalls.Request(ApiAddress.MuseumApiPath+ $"UpdateMuseumTilesWallId?wallId={_currentCardName}", headers,
					HttpClient.Method.Post, body);
			}
			else
			{
				MuseumActions.OnWallpaperSuccessfullyUpdated?.Invoke();
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		_httpRequestForUpdatingWalls.RequestCompleted -= HttpRequestForUpdatingWallsOnRequestCompleted;
		MuseumActions.OnClickBuilderCard -= OnClickBuilderCard;
		MuseumActions.OnClickWallForUpdatingWallPaper -= OnClickWallForUpdatingWallPaper;
	}
}
