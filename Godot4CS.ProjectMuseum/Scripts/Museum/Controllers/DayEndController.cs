using Godot;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;
using Time = ProjectMuseum.Models.Time;

public partial class DayEndController : Node2D
{
	[Export] private Vector2I _dayEndTileCoord = new Vector2I(0, -2);
	[Export] private int _saveSpotTileSourceId = 13;
	[Export] private int _forcedDayEndHour = 23;
	[Export] private int _forcedDayEndMinute = 30;
	[Export] private int _forcedDayEndWarningHour = 22;
	[Export] private int _forcedDayEndWarningMinute = 30;
	private bool _endingDay = false;

	private string _playerName = "";

	private HttpRequest _httpRequestForGettingPlayerInfo;
    // Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		_httpRequestForGettingPlayerInfo = new HttpRequest();
		AddChild(_httpRequestForGettingPlayerInfo);
		_httpRequestForGettingPlayerInfo.RequestCompleted += HttpRequestForGettingPlayerInfoOnRequestCompleted;
		_httpRequestForGettingPlayerInfo.Request(ApiAddress.PlayerApiPath + "GetPlayerInfo");
		MuseumActions.PlayerEnteredNewTile += PlayerEnteredNewTile;

		MuseumActions.OnMuseumTilesUpdated +=  SetDayEndCell;
		MuseumActions.OnTimeUpdated += OnTimeUpdated;
	}

	private void HttpRequestForGettingPlayerInfoOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
        string jsonStr = Encoding.UTF8.GetString(body);
		GD.Print($"Got player Name {jsonStr}");
		var playerInfo = JsonSerializer.Deserialize<PlayerInfo>(jsonStr);
		_playerName = playerInfo.Name;
		

	}

	private void OnTimeUpdated(int minutes, int hours, int days, int months, int years)
	{
		if (hours == _forcedDayEndHour && minutes == _forcedDayEndMinute &&! _endingDay)
		{
			_endingDay = true;
			SleepAndSave();
		}
		if (hours == _forcedDayEndWarningHour && minutes == _forcedDayEndWarningMinute &&! _endingDay)
		{
			MuseumActions.OnNeedOfWarning?.Invoke($"It is getting late. {_playerName} will sleep soon.");
		}
		if (_endingDay && hours == _forcedDayEndHour && minutes == _forcedDayEndMinute)
		{
			_endingDay = false;
		}
	}

	private void SetDayEndCell()
	{
		GameManager.tileMap.SetCell(0, _dayEndTileCoord, _saveSpotTileSourceId, new Vector2I(0, 0));
	}

	private void PlayerEnteredNewTile(Vector2I playerEnteredTile)
	{
		//GD.Print("Player Entered new Tile");
		if (playerEnteredTile == _dayEndTileCoord)
		{
			//GD.Print("Player Entered Day End Tile");
			MuseumActions.OnNeedOfPopUpUi?.Invoke("Are you sure you want to save and end the day?");
			MuseumActions.OnClickYesOfPopUpUi = OnClickYesOfPopUpUi;
			MuseumActions.OnClickNoOfPopUpUi = OnClickNoOfPopUpUi;
		}
	}

	private void OnClickNoOfPopUpUi()
	{
		MuseumActions.OnClickYesOfPopUpUi = null;
		MuseumActions.OnClickYesOfPopUpUi = null;
	}

	private void OnClickYesOfPopUpUi()
	{
		SleepAndSave();
		MuseumActions.OnClickYesOfPopUpUi = null;
		MuseumActions.OnClickYesOfPopUpUi = null;
		MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("SleepAndSave");
	}

	private static void SleepAndSave()
	{
		MuseumActions.DayEnded?.Invoke();
		MuseumActions.OnPlayerSavedGame?.Invoke();
		MuseumActions.OnPlayerSleepAndSavedGame?.Invoke();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForGettingPlayerInfo.RequestCompleted -= HttpRequestForGettingPlayerInfoOnRequestCompleted;
		MuseumActions.PlayerEnteredNewTile -= PlayerEnteredNewTile;

		MuseumActions.OnMuseumTilesUpdated -=  SetDayEndCell;
		MuseumActions.OnTimeUpdated -= OnTimeUpdated;

	}
}
