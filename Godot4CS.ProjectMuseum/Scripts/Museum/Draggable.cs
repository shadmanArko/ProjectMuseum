using Godot;
using System;

public partial class Draggable : ColorRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("Draggable");
		
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		GD.Print($"get_drag_data has run");
		return this;
	}
}
