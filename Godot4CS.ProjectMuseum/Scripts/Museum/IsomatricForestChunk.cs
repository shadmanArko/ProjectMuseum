using Godot;
using System;
using System.Collections.Generic;

public partial class IsomatricForestChunk : Sprite2D
{
	[Export] private Button _expansionButton;
	[Export] private Vector2I _expansionOrigin;
	// [Export] private List<Node2D> _neighbourChunks;
	[Export] private bool _firstNeighbourOfMuseum;
	[Export] public bool _alreadyExpanded;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// _expansionButton.
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
