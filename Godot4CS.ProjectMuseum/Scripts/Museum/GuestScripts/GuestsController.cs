using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class GuestsController : Node2D
{
	[Export] private PackedScene _guestScene;
	[Export] private Vector2I _spawnAtTile;
	private List<MuseumTile> _listOfMuseumTile;
	private string _testString;
	private HttpRequest _httpRequestForGettingMuseumTiles;

	private bool _isMuseumGateOpen = false;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		await Task.Delay(500);
		_listOfMuseumTile = ServiceRegistry.Resolve<List<MuseumTile>>();
		
		_httpRequestForGettingMuseumTiles = new HttpRequest();
		AddChild(_httpRequestForGettingMuseumTiles);
		_httpRequestForGettingMuseumTiles.RequestCompleted += HttpRequestForGettingMuseumTilesOnRequestCompleted;
		_httpRequestForGettingMuseumTiles.Request(ApiAddress.MuseumApiPath + "GetAllMuseumTiles");
		MuseumActions.OnClickMuseumGateToggle += OnClickMuseumGateToggle;
		await Task.Delay(500);
	}

	private void OnClickMuseumGateToggle(bool gateOpen)
	{
		_isMuseumGateOpen = gateOpen;
		if (gateOpen)
		{
			SpawnGuests(10, 5);
		}
	}

	private async void SpawnGuests(int numberOfGuests , float delayBetweenSpawningGuests)
	{
		for (int i = 0; i < numberOfGuests; i++)
		{
			if (!_isMuseumGateOpen) return;
			var guest = _guestScene.Instantiate();
			guest.GetNode<Guest>(".").Position = GameManager.TileMap.MapToLocal(_spawnAtTile);
			AddChild(guest);
			guest.GetNode<Guest>(".").Initialize(_listOfMuseumTile);
			await Task.Delay( (int)(1000 * delayBetweenSpawningGuests));
		}
	}
	private void HttpRequestForGettingMuseumTilesOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_listOfMuseumTile = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		if (_listOfMuseumTile.Count > 0)
		{
			GD.Print($"found museum tiles {_listOfMuseumTile.Count}");
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	
}
