using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class FadeInAndOutUi : Control
{
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Label _timeUi;

	private int _lastSavedDay;
	private int _lastSavedMonth;
	private int _lastSavedYear;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnPlayerSleepAndSavedGame += PlayFadeInAndOut;
		MuseumActions.OnTimeUpdated += OnTimeUpdated;
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
		_animationPlayer.Play("Fade_In");
		await Task.Delay(1000);
		await Task.Delay(700);
		ChangeDay();
		await Task.Delay(1000);
		_animationPlayer.Play("Fade_Out");
	}

	private void ChangeDay()
	{
		MuseumActions.OnSleepComplete?.Invoke();
	}

	public override void _ExitTree()
	{
		MuseumActions.OnPlayerSleepAndSavedGame -= PlayFadeInAndOut;
		MuseumActions.OnTimeUpdated -= OnTimeUpdated;
	}
}
