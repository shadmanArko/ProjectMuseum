using Godot;
using System;
using Godot.Collections;

public partial class ExhibitEditor2x2Grid : GridContainer
{
	[Export] private Array<DropTarget> _dropTargets;
	[Export] private int _gridNumber;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
