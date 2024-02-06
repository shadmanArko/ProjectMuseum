using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class DayNightCycleController : Node2D
{
	private int _nightColor = 0x7cb4f6;
	private int _eveningColor = 0xf7ab8a;
	private int _dayColor = 0xffffff;
	private int _zoomInColor = 0x464646;
	[Export] private Node2D _backgroundObject;
	private double _elapsedTime = 0;
	private float _startTransitionDuration = 10f;
	private float _transitionDuration = 10f;

	private bool _transitionOn = false;

	private Color _transitionStartColor;
	private Color _transitionEndColor;

	private Color _currentFixedColor;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_transitionOn)
		{
			// Update the elapsed time
			_elapsedTime += delta;

			// Calculate the interpolation factor
			float t = (float)Mathf.Min(_elapsedTime / _transitionDuration, 1.0f);

			var startColor = _transitionStartColor;
			var targetColor = _transitionEndColor;
			// Interpolate between startColor and endColor
			Color currentColor = startColor.Lerp(targetColor, t);

			// Apply the interpolated color to your node or use it as needed
			_backgroundObject.Modulate = currentColor;
		}
		else
		{
			_backgroundObject.Modulate = _currentFixedColor;
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
	private void OnTimeUpdated(int minutes, int hours, int days, int months, int years)
	{
		_elapsedTime = 0;
		float timeInFraction = GetTimeInFraction(hours, minutes);
		if (timeInFraction is >= 6 and <= 16)
		{
			//day
			_transitionOn = false;
			_currentFixedColor = GetColor(_dayColor);
		}else if (timeInFraction >= 16 && hours < 17)
		{
			//day to evening transition
			_transitionOn = true;
			_transitionStartColor = _backgroundObject.Modulate;
			var endColor = GetColor(_eveningColor);
			_transitionEndColor = _transitionStartColor.Lerp(endColor, timeInFraction - hours);
		}
		else if (Math.Abs(timeInFraction - 17) < 0.1f)
		{
			//final color of evening
			_transitionOn = true;
			_transitionStartColor = _backgroundObject.Modulate;
			_transitionEndColor =  GetColor(_eveningColor);
		}else if (timeInFraction is >= 17 and <= 18)
		{
			//evening
			_transitionOn = false;
			_currentFixedColor = GetColor(_eveningColor);
		}
		else if (timeInFraction >= 18 && hours < 19)
		{
			//evening to night
			_transitionOn = true;
			_transitionStartColor = _backgroundObject.Modulate;
			var endColor = GetColor(_nightColor);
			_transitionEndColor = _transitionStartColor.Lerp(endColor, timeInFraction - hours);
		}
		else if (Math.Abs(timeInFraction - 19) < 0.1f)
		{
			_transitionOn = true;
			_transitionStartColor = _backgroundObject.Modulate;
			_transitionEndColor =  GetColor(_nightColor);
		}else if (timeInFraction is >= 19 or <= 6)
		{
			//night	
			_transitionOn = false;
			_currentFixedColor = GetColor(_nightColor);
		}
	}

	private float GetTimeInFraction(int hours, int minutes)
	{
		float time = hours + (minutes / 60f);
		return time;
	}
	private void OnClickTimeSpeedButton(int speed)
	{
		_transitionDuration = _startTransitionDuration / speed;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		MuseumActions.OnTimeUpdated += OnTimeUpdated;
		MuseumActions.OnClickTimeSpeedButton += OnClickTimeSpeedButton;
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnTimeUpdated -= OnTimeUpdated;
		MuseumActions.OnClickTimeSpeedButton -= OnClickTimeSpeedButton;
	}
}
