using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine;
using Time = ProjectMuseum.Models.Time;

namespace Godot4CS.ProjectMuseum.Plugins.Time_System.MineTimeSystem.Scripts;

public partial class MineTimeSystem : Node
{
	private double _secondsIn10Minutes = 5;
	private int _minutesInHour = 60;
	private int _hoursInDay = 24;
	private int _daysInMonth = 30;
	private int _monthsInYear = 4;

	[Export] private bool _isPaused;
    
	private  Time _time = new();

	public override void _EnterTree()
	{
		SubscribeToActions();
		_isPaused = true;
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerSpawned += StartDayOneMineExcavation;
	}

	public override void _Ready()
	{
		
	}

	
	public override void _Process(double delta)
	{ 
		if (!_isPaused)
		{
			_time.Seconds += delta;
    
			if (_time.Seconds >= _secondsIn10Minutes)
			{
				_time.Seconds = 0f;
				UpdateTime();
				MineActions.OnTenMinutesPassed?.Invoke(_time.Minutes);
			}
		}
	}
	
	private void UpdateTime()
	{
		_time.Minutes+=10;
		if (_time.Minutes >= _minutesInHour)
		{
			_time.Minutes = 0;
				_time.Hours++;
            MineActions.OnOneHourPassed?.Invoke(_time.Hours);
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
			
			if (_time.Hours >= _hoursInDay)
			{
				_time.Hours = 0;
                _time.Days++;
                MineActions.OnOneDayPassed?.Invoke(_time.Days);
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
				
				if (_time.Days >= _daysInMonth)
				{
					_time.Days = 1;
					_time.Months = (_time.Months % _monthsInYear) + 1;
					
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
		MineActions.OnTimeUpdated?.Invoke(_time.Minutes, _time.Hours, _time.Days, _time.Months, _time.Years);
	}

	public void StartDayOneMineExcavation()
	{
		_time.Days = 1;
		_time.Hours = 8;
		_time.Minutes = 0;
		_time.Seconds = 0;
		MineActions.OnTimeUpdated?.Invoke(_time.Minutes, _time.Hours, _time.Days, _time.Months, _time.Years);
	}
	
	public void StartNextDayMineExcavation()
	{
		++_time.Days;
		_time.Hours = 8;
		_time.Minutes = 0;
		_time.Seconds = 0;
		MineActions.OnTimeUpdated?.Invoke(_time.Minutes, _time.Hours, _time.Days, _time.Months, _time.Years);
		MineActions.OnOneDayPassed?.Invoke(_time.Days);
	}
	
	public void SetTime(int seconds, int minutes, int hours, int days)
	{
		_time.Days = days;
		_time.Hours = hours;
		_time.Minutes = minutes;
		_time.Seconds = seconds;
	}

	public Time GetTime()
	{
		return _time;
	}

	public void PlayTimer() => _isPaused = false;
	public void PauseTimer() => _isPaused = true;

	public void TogglePause()
	{
		_isPaused = !_isPaused;
	}
}