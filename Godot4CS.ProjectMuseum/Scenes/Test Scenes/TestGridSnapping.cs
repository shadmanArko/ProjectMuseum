using Godot;
using System;

public partial class TestGridSnapping : Control
{
	[Export] private PackedScene _item;
	[Export] private GridContainer GridContainer;
	private Control _currentItem;

	private bool placedItem = false;
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_currentItem = _item.Instantiate<Control>();
		AddChild(_currentItem);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_currentItem != null && !placedItem)
		{
			_currentItem.GlobalPosition = GetGlobalMousePosition();
		}

		if (Input.IsActionJustPressed("ui_left_click"))
		{
			_currentItem.Reparent(GridContainer);
			_currentItem.GlobalPosition = GetGlobalMousePosition();
			placedItem = true;
		}
		
	}
}
