using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Time = ProjectMuseum.Models.Time;

public partial class DayEndController : Node2D
{
	[Export] private Vector2I _dayEndTileCoord = new Vector2I(0, -2);
	[Export] private int _saveSpotTileSourceId = 13;
	[Export] private int _forcedDayEndHour = 23;
	[Export] private int _forcedDayEndMinute = 30;

	private bool _endingDay = false;
    // Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		MuseumActions.PlayerEnteredNewTile += PlayerEnteredNewTile;

		MuseumActions.OnMuseumTilesUpdated +=  SetDayEndCell;
		MuseumActions.OnTimeUpdated += OnTimeUpdated;
	}

	private void OnTimeUpdated(int minutes, int hours, int days, int months, int years)
	{
		if (hours == _forcedDayEndHour && minutes == _forcedDayEndMinute &&! _endingDay)
		{
			_endingDay = true;
			SleepAndSave();
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
		MuseumActions.PlayerEnteredNewTile -= PlayerEnteredNewTile;
		MuseumActions.OnMuseumTilesUpdated -=  SetDayEndCell;

	}
}
