using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class FadeInAndOutUi : Control
{
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Label _timeUi;
	[Export] private Control _dayEndReportPanel;
	[Export] private Button _dayEndReportClosingButton;
	private bool _conceptEnded = false;
	private int _lastSavedDay;
	private int _lastSavedMonth;
	private int _lastSavedYear;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnPlayerSleepAndSavedGame += PlayFadeInAndOut;
		MuseumActions.OnTimeUpdated += OnTimeUpdated;
		_dayEndReportClosingButton.Pressed += DayEndReportClosingButtonOnPressed;
		MuseumActions.OnConceptStoryCompleted += OnConceptStoryCompleted;
	}

	private void OnConceptStoryCompleted()
	{
		_conceptEnded = true;
	}

	private async void DayEndReportClosingButtonOnPressed()
	{
		_dayEndReportPanel.Visible = false;
		await Task.Delay(200);
		_timeUi.Visible = true;
		await Task.Delay(1000);
		ChangeDay();
		await Task.Delay(1000);
		_animationPlayer.Play("Fade_Out");
	}

	private void OnTimeUpdated(int minutes, int hours, int days, int months, int years)
	{
		_timeUi.Text = $"Day {days} Month {months} Year {years}";
		_lastSavedDay = days;
		_lastSavedMonth = months;
		_lastSavedYear = years;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	async void PlayFadeInAndOut()
	{
		if (_conceptEnded)
		{
			_animationPlayer.Play("Fade_In");
		}
		else
		{
			_animationPlayer.Play("Fade_In_Without_Report");
			await Task.Delay(1500);
			ChangeDay();
			await Task.Delay(1000);
			_animationPlayer.Play("Fade_Out");
		}

		
	}

	private void ChangeDay()
	{
		MuseumActions.OnSleepComplete?.Invoke();
	}

	public override void _ExitTree()
	{
		MuseumActions.OnPlayerSleepAndSavedGame -= PlayFadeInAndOut;
		MuseumActions.OnTimeUpdated -= OnTimeUpdated;
		_dayEndReportClosingButton.Pressed -= DayEndReportClosingButtonOnPressed;
		MuseumActions.OnConceptStoryCompleted -= OnConceptStoryCompleted;

	}
}
