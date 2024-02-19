using Godot;
using System;

public partial class ControllersContainer : Node2D
{
	private string _pathToTileSelectorScene = "res://Scenes/Museum/Sub Scenes/TileSelector/TileSelector.tscn";
	[Export] private Node2D _controllersParent;

	private Node _tileSelectorInstance; // Variable to store the instantiated tile selector scene

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InstantiateController();
	}

	private void InstantiateController()
	{
		if (_tileSelectorInstance == null && _controllersParent != null)
		{
			var tileSelectorScene = (PackedScene)ResourceLoader.Load(_pathToTileSelectorScene);
			if (tileSelectorScene != null)
			{
				_tileSelectorInstance = tileSelectorScene.Instantiate() as Node; // Instantiate the scene
				if (_tileSelectorInstance != null)
				{
					_controllersParent.AddChild(_tileSelectorInstance); // Add it as a child of the _controllersParent
				}
			}
			else
			{
				GD.PrintErr("Failed to load tile selector scene from path: " + _pathToTileSelectorScene);
			}
		}
		else
		{
			GD.PrintErr("Tile selector scene is already instantiated or controllers parent is not assigned.");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Process logic here if needed
	}
}
