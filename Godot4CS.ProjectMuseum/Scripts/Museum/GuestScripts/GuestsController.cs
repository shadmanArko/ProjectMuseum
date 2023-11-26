using Godot;
using System;

public partial class GuestsController : Node2D
{
	[Export] private PackedScene _guestScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < 5; i++)
		{
			var guest = _guestScene.Instantiate();
			guest.GetNode<Guest>(".").Position = new Vector2(54, -120); 
			AddChild(guest);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	
}
