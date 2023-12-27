using Godot;
using System;
using System.Text;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using Time = ProjectMuseum.Models.Time;
using JsonSerializer = System.Text.Json.JsonSerializer;
public partial class MuseumTimeSystem : Node
{
	private double _secondsIn10Minutes = 5;
	private int _minutesInHour = 60;
	private int _hoursInDay = 24;
	private int _daysInMonth = 30;
	private int _monthsInYear = 4;

	private bool _isPaused = false;

    private double _originalClockUnitSpeed;
    private  Time _time = new Time();
    private HttpRequest _httpRequestForGettingTime;
    private HttpRequest _httpRequestForUpdatingTime;
    public override void _Ready()
	{
		_originalClockUnitSpeed = _secondsIn10Minutes;

		_httpRequestForGettingTime = new HttpRequest();
		_httpRequestForUpdatingTime = new HttpRequest();
		AddChild(_httpRequestForGettingTime);
		AddChild(_httpRequestForUpdatingTime);
		_httpRequestForGettingTime.RequestCompleted += HttpRequestForGettingTimeOnRequestCompleted;
		_httpRequestForUpdatingTime.RequestCompleted += HttpRequestForUpdatingTimeOnRequestCompleted;
		_httpRequestForGettingTime.Request(ApiAddress.PlayerApiPath + "GetTime");
		// MuseumActions.OnTimeUpdated?.Invoke(_time.Minutes, _time.Hours, _time.Days, _time.Months, _time.Years);
		MuseumActions.OnClickTimeSpeedButton += SetClockSpeed;
		MuseumActions.OnClickPausePlayButton += TogglePause;
		MuseumActions.OnPlayerSavedGame += SaveTime;
	}

    private void HttpRequestForUpdatingTimeOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
	    string jsonStr = Encoding.UTF8.GetString(body);
	    GD.Print( "saved time "+jsonStr);
    }

    private void HttpRequestForGettingTimeOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
	    string jsonStr = Encoding.UTF8.GetString(body);
	    GD.Print(jsonStr);
	    _time = JsonSerializer.Deserialize<Time>(jsonStr);
	    MuseumActions.OnTimeUpdated?.Invoke(_time.Minutes, _time.Hours, _time.Days, _time.Months, _time.Years);
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
                }
            }
	}
	
	private void UpdateTime()
	{
		// GD.Print($"Season: {_time.Months}, Day: {_time.Days}, Time: {_time.Hours:D2}:{_time.Minutes:D2}");
		_time.Minutes+=10;
		if (_time.Minutes >= _minutesInHour)
		{
			_time.Minutes = 0;
			_time.Hours++;
			
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
					_time.Months++;
					// _months = (_months % _monthsInYear) + 1;

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
					if (_time.Months >_monthsInYear)
					{
						_time.Months = 1;
						_time.Years++;
					}
				}
			}
		}
		MuseumActions.OnTimeUpdated?.Invoke(_time.Minutes, _time.Hours, _time.Days, _time.Months, _time.Years);
	}

	private void SaveTime()
	{
		string[] headers = { "Content-Type: application/json"};
		var body = JsonConvert.SerializeObject(_time);
		string url = ApiAddress.PlayerApiPath + "SaveTime";
		// _httpRequestForExhibitPlacement.Request(url, headers, HttpClient.Method.Get, body);
		_httpRequestForUpdatingTime.Request(url, headers, HttpClient.Method.Post, body);
	}

	public void TogglePause()
	{
		_isPaused = !_isPaused;
		MuseumActions.OnTimePauseValueUpdated?.Invoke(_isPaused);
		GD.Print(_isPaused ? "Game Paused" : "Game Resumed");
	}
	public void SetClockSpeed(int speed)
	{
		_secondsIn10Minutes = _originalClockUnitSpeed / speed;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnClickTimeSpeedButton -= SetClockSpeed;
		MuseumActions.OnClickPausePlayButton -= TogglePause;
		MuseumActions.OnPlayerSavedGame -= SaveTime;
	}
}
