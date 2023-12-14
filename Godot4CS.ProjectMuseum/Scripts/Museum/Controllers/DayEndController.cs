using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;

public partial class DayEndController : Node2D
{
	[Export] private Vector2I _dayEndTileCoord = new Vector2I(0, -2);
	[Export] private int _saveSpotTileSourceId = 13;
    // Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		MuseumActions.PlayerEnteredNewTile += PlayerEnteredNewTile;
		await Task.Delay(1000);
		GameManager.TileMap.SetCell(0, _dayEndTileCoord, _saveSpotTileSourceId, new Vector2I(0, 0));
	}

	private void PlayerEnteredNewTile(Vector2I playerEnteredTile)
	{
		GD.Print("Player Entered new Tile");
		if (playerEnteredTile == _dayEndTileCoord)
		{
			GD.Print("Player Entered Day End Tile");
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
		MuseumActions.OnPlayerSavedGame?.Invoke();
		MuseumActions.OnClickYesOfPopUpUi = null;
		MuseumActions.OnClickYesOfPopUpUi = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.PlayerEnteredNewTile -= PlayerEnteredNewTile;
	}
}
