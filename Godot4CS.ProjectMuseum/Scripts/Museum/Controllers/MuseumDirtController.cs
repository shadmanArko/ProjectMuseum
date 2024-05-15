using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class MuseumDirtController : Node2D
{
	private int _lastCheckedHour = 0;
	// Called when the node enters the scene tree for the first time.
	private MuseumTileContainer _museumTileContainer;
	public override async void _Ready()
	{
		MuseumActions.OnTimeUpdated += OnTimeUpdated;
		await Task.Delay(1000);
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
	}

	private void OnTimeUpdated(int minutes, int hours, int days, int months, int years)
	{
		if (_lastCheckedHour != 0 && _lastCheckedHour != hours)
		{
		 	var tile = _museumTileContainer.MuseumTiles[GD.RandRange(0, _museumTileContainer.MuseumTiles.Count - 1)];
		    GameManager.tileMap.SetCell(2, new Vector2I(tile.XPosition, tile.YPosition), 14, Vector2I.Zero);
		}

		_lastCheckedHour = hours;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnTimeUpdated -= OnTimeUpdated;

	}
}
