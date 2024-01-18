using Godot;
using System;

public partial class DayNightCycleController : Node2D
{
	private int _nightColor = 0x7cb4f6;
	private int _eveningColor = 0xf7ab8a;
	private int _dayColor = 0xffffff;
	private int _zoomInColor = 0x464646;
	[Export] private Node2D _backgroundObject;
	private double _elapsedTime = 0;
	private float _transitionDuration = 10f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Update the elapsed time
		_elapsedTime += delta;

		// Calculate the interpolation factor
		float t = (float)Mathf.Min(_elapsedTime / _transitionDuration, 1.0f);

		var startColor = GetColor(_dayColor);
		var targetColor = GetColor(_eveningColor);
		// Interpolate between startColor and endColor
		Color currentColor = startColor.Lerp(targetColor, t);

		// Apply the interpolated color to your node or use it as needed
		 _backgroundObject.Modulate = currentColor;

		// Reset the elapsed time after the transition is complete
		if (t >= 1.0f)
		{
			_elapsedTime = 0.0f;
			_dayColor = _eveningColor;
			_eveningColor = _nightColor;
		}
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
	private Color GetColor(int hexCode)
	{
		// int hexCode = 0xFF00FF; // Replace this with your hex code
		int red = (hexCode >> 16) & 0xFF;
		int green = (hexCode >> 8) & 0xFF;
		int blue = hexCode & 0xFF;
		// Color currentColor = color.Lerp(endColor, )
		// Set the modulate color
		Color modulateColor = new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
		return modulateColor;
	}
}
