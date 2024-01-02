using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class TownBuilding : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	private bool _mouseOnBuilding;
	private Color _startColor;
	[Export] private bool _hasDiggingBuddy;
	public override void _Ready()
	{
		_startColor = Modulate;
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
			Modulate = Colors.Brown;
			if (_hasDiggingBuddy)
			{
				MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("FoundDiggingBuddy");
			}
		}
	}


	private void OnMouseEntered()
	{
		_mouseOnBuilding = true;
		Modulate = Colors.Burlywood;
		GD.Print("Mouse Entered"+ Name);
	}
	private void OnMouseExit()
	{
		_mouseOnBuilding = false;
		Modulate = _startColor;
		GD.Print("Mouse Exit" + Name);
	}
}
