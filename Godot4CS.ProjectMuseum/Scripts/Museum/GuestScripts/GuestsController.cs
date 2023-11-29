using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;

public partial class GuestsController : Node2D
{
	[Export] private PackedScene _guestScene;
	[Export] private Vector2I _spawnAtTile;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		await Task.Delay(500);
		await SpawnGuests(10, 5);
	}

	private async Task SpawnGuests(int numberOfGuests , float delayBetweenSpawningGuests)
	{
		for (int i = 0; i < numberOfGuests; i++)
		{
			var guest = _guestScene.Instantiate();
			guest.GetNode<Guest>(".").Position = GameManager.TileMap.MapToLocal(_spawnAtTile);
			AddChild(guest);
			guest.GetNode<Guest>(".").Initialize();
			await Task.Delay( (int)(1000 * delayBetweenSpawningGuests));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	
}
