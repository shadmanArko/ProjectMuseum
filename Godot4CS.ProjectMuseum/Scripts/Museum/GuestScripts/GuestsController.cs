using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class GuestsController : Node2D
{
	[Export] private PackedScene _guestScene;
	[Export] private Vector2I _spawnAtTile;
	[Export] private double _spawnInterval = 4.0;
	[Export] private Array<Vector2I> _sceneEntryPositions;
	private MuseumTileContainer _museumTileContainer;
	private string _testString;
	private HttpRequest _httpRequestForGettingMuseumTiles;
	[Export] private int _maxNumberGuests = 10;
	private int _numberOfGuestsInMuseum;
	private int _numberOfPeopleInScene;
	private int _maxNumberOfPeopleInTheScene = 5000;
	private bool _isMuseumGateOpen = false;
	private bool _isGamePaused = false;
	private float _ticketPrice = 5;
    
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		await Task.Delay(500);
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
		
		_httpRequestForGettingMuseumTiles = new HttpRequest();
		AddChild(_httpRequestForGettingMuseumTiles);
		_httpRequestForGettingMuseumTiles.RequestCompleted += HttpRequestForGettingMuseumTilesOnRequestCompleted;
		MuseumActions.OnClickMuseumGateToggle += OnClickMuseumGateToggle;
		MuseumActions.OnTimePauseValueUpdated += OnTimePauseValueUpdated;
		MuseumActions.OnGuestExitMuseum += OnGuestExitMuseum;
		MuseumActions.OnGuestEnterMuseum += OnGuestEnterMuseum;
		MuseumActions.OnGuestExitScene += OnGuestExitScene;
		await Task.Delay(500);
		_httpRequestForGettingMuseumTiles.Request(ApiAddress.MuseumApiPath + "GetAllMuseumTiles");

	}

	private void OnGuestExitScene()
	{
		_numberOfPeopleInScene--;
	}

	private void OnGuestExitMuseum()
	{
		_numberOfGuestsInMuseum--;
		MuseumActions.TotalGuestsUpdated?.Invoke(_numberOfGuestsInMuseum);
	}
	private void OnGuestEnterMuseum()
	{
		_numberOfGuestsInMuseum++;
		MuseumActions.TotalGuestsUpdated?.Invoke(_numberOfGuestsInMuseum);
		MuseumActions.OnMuseumBalanceAdded?.Invoke(_ticketPrice);
	}

	private double _timer;
	public override void _Process(double delta)
	{
		_timer += delta;
		if (_numberOfPeopleInScene < _maxNumberOfPeopleInTheScene && _timer>= _spawnInterval)
		{
			_timer = 0;
			SpawnGuest();
			_numberOfPeopleInScene++;
			// GD.Print($"Fps {Engine.GetFramesPerSecond()} for {_numberOfPeopleInScene} ");
		}
	}

	private void OnTimePauseValueUpdated(bool obj)
	{
		_isGamePaused = obj;
		if (!_isGamePaused && _isMuseumGateOpen)
		{
			SpawnGuests(10 -_numberOfGuestsInMuseum, 5);
		}
	}

	private void OnClickMuseumGateToggle(bool gateOpen)
	{
		_isMuseumGateOpen = gateOpen;
		GameManager.isMuseumGateOpen = gateOpen;
		// if (gateOpen)
		// {
		// 	SpawnGuests(10 -_numberOfGuestsInMuseum, 5);
		// }
	}

	private async void SpawnGuests(int numberOfGuests , float delayBetweenSpawningGuests)
	{
		for (int i = 0; i < numberOfGuests; i++)
		{
			if (!_isMuseumGateOpen || _numberOfGuestsInMuseum >= _maxNumberGuests || _isGamePaused) return;
			SpawnGuest();
			_numberOfGuestsInMuseum++;
			MuseumActions.OnMuseumBalanceAdded?.Invoke(_ticketPrice);
			MuseumActions.TotalGuestsUpdated?.Invoke(_numberOfGuestsInMuseum);
			await Task.Delay( (int)(1000 * delayBetweenSpawningGuests));
		}
	}

	private void SpawnGuest()
	{
		var guest = _guestScene.Instantiate();
		guest.GetNode<Guest>(".").Position = GameManager.tileMap.MapToLocal(_sceneEntryPositions[GD.RandRange(0, _sceneEntryPositions.Count-1)]);
		AddChild(guest);
		guest.GetNode<Guest>(".").Initialize(_sceneEntryPositions.ToList());
	}

	private void HttpRequestForGettingMuseumTilesOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_museumTileContainer.MuseumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		if (_museumTileContainer.MuseumTiles.Count > 0)
		{
			//GD.Print($"found museum tiles {_museumTileContainer.MuseumTiles.Count}");
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForGettingMuseumTiles.RequestCompleted -= HttpRequestForGettingMuseumTilesOnRequestCompleted;
		MuseumActions.OnClickMuseumGateToggle -= OnClickMuseumGateToggle;
		MuseumActions.OnTimePauseValueUpdated -= OnTimePauseValueUpdated;
		MuseumActions.OnGuestExitMuseum -= OnGuestExitMuseum;
		MuseumActions.OnGuestEnterMuseum -= OnGuestEnterMuseum;
		MuseumActions.OnGuestExitScene -= OnGuestExitScene;

	}
	
}
