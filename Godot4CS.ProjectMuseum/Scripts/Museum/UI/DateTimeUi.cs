using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class DateTimeUi : Control
{
	[Export] private Label _clockTime;
	[Export] private Label _dayName;
	[Export] private Label _day;
	[Export] private Label _month;
	[Export] private TextureRect _monthIcon;
	[Export] private Label _year;
	[Export] private Button _pausePlayButton;
	[Export] private Button _timeSpeed1X;
	[Export] private Button _timeSpeed2X;
	[Export] private Button _timeSpeed4X;
	[Export] private Texture2D _spring;
	[Export] private Texture2D _summer;
	[Export] private Texture2D _autumn;
	[Export] private Texture2D _winter;
	// [Export] private Button _timeSpeed8X;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        MuseumActions.OnTimeUpdated += OnTimeUpdated;
        _pausePlayButton.Pressed += PausePlayButtonOnPressed;
        _timeSpeed1X.Pressed += TimeSpeed1xOnPressed;
        _timeSpeed2X.Pressed += TimeSpeed2xOnPressed;
        _timeSpeed4X.Pressed += TimeSpeed4xOnPressed;
        // _timeSpeed8X.Pressed += TimeSpeed8xOnPressed;
	}

	private void TimeSpeed8xOnPressed()
	{
		MuseumActions.OnClickTimeSpeedButton?.Invoke(8);
	}

	private void TimeSpeed4xOnPressed()
	{
		MuseumActions.OnClickTimeSpeedButton?.Invoke(4);
	}

	private void TimeSpeed2xOnPressed()
	{
		MuseumActions.OnClickTimeSpeedButton?.Invoke(2);
	}

	private void TimeSpeed1xOnPressed()
	{
		MuseumActions.OnClickTimeSpeedButton?.Invoke(1);
	}

	private void PausePlayButtonOnPressed()
	{
		MuseumActions.OnClickPausePlayButton?.Invoke();
	}

	private void OnTimeUpdated(int minutes, int hours, int days, int months, int years)
	{
		// _dateTime.Text = $"{hours:D2}:{minutes:D2} {days.ToCorrespondedDay()} {days:d2}/ {months:d2}/ Year {years:d2}";
		_clockTime.Text = $"{hours:D2}:{minutes:D2}";
		_dayName.Text = $"{days.ToCorrespondedDayShort()}";
		_day.Text = $"{days:d2}";
		// _month.Text = $"{months:d2}";
		_monthIcon.Texture = GetMonthIcon(months);
		_year.Text = $"Yr {years:d2}";
	}

	private Texture2D GetMonthIcon(int months)
	{
		if (months == 1)
		{
			return _spring;
		}else if (months == 2)
		{
			return _summer;
		}else if (months == 3)
		{
			return _autumn;
		}else if (months == 4)
		{
			return _winter;
		}

		return _spring;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		MuseumActions.OnTimeUpdated -= OnTimeUpdated;
	}
}
