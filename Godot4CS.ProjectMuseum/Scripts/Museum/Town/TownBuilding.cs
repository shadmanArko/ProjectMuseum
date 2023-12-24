using Godot;
using System;

public partial class TownBuilding : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	private bool _mouseOnBuilding;
	public override void _Ready()
	{
		GD.Print("Building started");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_left_click") && _mouseOnBuilding)
		{
			GD.Print("Mouse Clicked"+ Name);
		}
	}


	private void OnMouseEntered()
	{
		_mouseOnBuilding = true;
		GD.Print("Mouse Entered"+ Name);
	}
	private void InputEvent(Viewport viewport, InputEvent @event, int index)
	{
	}
	private void OnMouseExit()
	{
		_mouseOnBuilding = false;
		GD.Print("Mouse Exit" + Name);
	}
}
