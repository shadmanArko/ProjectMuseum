using Godot;
using System;

public partial class MuseumTimeSystem : Node
{
	private double _secondsIn10Minutes = 5;
	private int _minutesInHour = 60;
	private int _hoursInDay = 24;
	private int _daysInMonth = 30;
	private int _monthsInYear = 4;

	private double _seconds = 0;
	private int _minutes = 0;
	private int _hours = 0;
	private int _days = 1;
	private int _months = 1;
	
    private bool _isPaused = false;
    
    
	public override void _Ready()
	{
		
	}

	
	public override void _Process(double delta)
	{ 
			if (!_isPaused)
            {
                _seconds += delta;
    
                if (_seconds >= _secondsIn10Minutes)
                {
                    _seconds = 0f;
                    UpdateTime();
                }
            }
	}
	
	private void UpdateTime()
	{
		GD.Print($"Season: {_months}, Day: {_days}, Time: {_hours:D2}:{_minutes:D2}");
		
		_minutes+=10;
		if (_minutes >= _minutesInHour)
		{
			_minutes = 0;
			_hours++;
			
			// TODO Invoke Hourly Events
			
			// <example>
			// This example demonstrates how to use the Add method.
			// <code>
			// if (_hours == 8)
			// {
			// 	Action1.Invoke();
			// }
			// </code>
			// </example>
			
			if (_hours >= _hoursInDay)
			{
				_hours = 0;
				_days++;
				
				//TODO Invoke Daily Events
				
				// <example>
				// This example demonstrates how to use the Add method.
				// <code>
				// if (_days == 15)
				// {
				// 	Action1.Invoke();
				// }
				// </code>
				// </example>
				
				if (_days >= _daysInMonth)
				{
					_days = 1;
					_months = (_months % _monthsInYear) + 1;
					
					//TODO Invoke Monthly Events
					
					// <example>
					// This example demonstrates how to use the Add method.
					// <code>
					// if (_months == 2)
					// {
					// 	Action1.Invoke();
					// }
					// </code>
					// </example>
				}
			}
		}
	}
	

	public void TogglePause()
	{
		_isPaused = !_isPaused;
		GD.Print(_isPaused ? "Game Paused" : "Game Resumed");
	}
}
