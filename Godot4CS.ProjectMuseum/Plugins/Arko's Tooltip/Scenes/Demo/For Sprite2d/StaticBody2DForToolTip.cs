using Godot;
using System;

public partial class StaticBody2DForToolTip : StaticBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnMouseEntered()
	{
		GD.Print("Mouse is entered into static body");
	}
}
