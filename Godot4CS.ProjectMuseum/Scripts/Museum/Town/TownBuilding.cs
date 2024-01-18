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
		//GD.Print("Building started");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_left_click") && _mouseOnBuilding)
		{
			//GD.Print("Mouse Clicked"+ Name);
			// Modulate = Colors.Brown;
			AssignColor(0x808080);
			if (_hasDiggingBuddy)
			{
				MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("FoundDiggingBuddy");
			}
			else
			{
				MuseumActions.OnPlayerClickedAnEmptyHouse?.Invoke();
			}
		}
	}


	private void OnMouseEntered()
	{
		_mouseOnBuilding = true;
		AssignColor(0xD3D3D3);
		var color = Colors.Brown;
		var endColor = Colors.Black;

		// Modulate = Colors.LightBlue;
		//GD.Print("Mouse Entered"+ Name);
	}
	private void OnMouseExit()
	{
		_mouseOnBuilding = false;
		Modulate = _startColor;
		//GD.Print("Mouse Exit" + Name);
	}
	private void AssignColor(int hexCode)
	{
		// int hexCode = 0xFF00FF; // Replace this with your hex code
		int red = (hexCode >> 16) & 0xFF;
		int green = (hexCode >> 8) & 0xFF;
		int blue = hexCode & 0xFF;
		// Color currentColor = color.Lerp(endColor, )
		// Set the modulate color
		Color modulateColor = new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
		Modulate = modulateColor;
	}
}
