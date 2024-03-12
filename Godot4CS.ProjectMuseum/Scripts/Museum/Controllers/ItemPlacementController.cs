using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class ItemPlacementController : Node2D
{
	[Export] private PackedScene _placementEffect1X1;
	[Export] private PackedScene _placementEffect2X2;
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnItemPlacedOnTile += OnItemPlacedOnTile;
	}

	private async void OnItemPlacedOnTile(Item item, Vector2 position)
	{
		
		for (int i = 0; i < 1; i++)
		{
			item.RotationDegrees = 5;
			await Task.Delay(50);
			item.RotationDegrees = -5;
			await Task.Delay(50);
		}
		item.RotationDegrees = 0;
		item.MakeObjectsGrounded();
		if (item.numberOfTilesItTakes == 1)
		{
			var instance = InstantiateEffect(_placementEffect1X1, position);
		}
		else if (item.numberOfTilesItTakes ==4)
		{
			var instance = InstantiateEffect(_placementEffect2X2, position);
		}
	}

	private Node2D InstantiateEffect(PackedScene packedScene, Vector2 position)
	{
		var instance = (Node2D)packedScene.Instantiate();
		AddChild(instance);
		instance.Position = (position + new Vector2(0, 1f));
		return instance;
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
