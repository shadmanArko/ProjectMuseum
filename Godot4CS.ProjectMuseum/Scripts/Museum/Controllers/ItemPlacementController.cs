using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class ItemPlacementController : Node2D
{
	[Export] private PackedScene _placementEffect1X1;
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnItemPlacedOnTile += OnItemPlacedOnTile;
	}

	private void OnItemPlacedOnTile(Item item, Vector2 position)
	{
		var instance = (Node2D)_placementEffect1X1.Instantiate();
		AddChild(instance);
		instance.Position = position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnItemPlacedOnTile -= OnItemPlacedOnTile;
	}
}
