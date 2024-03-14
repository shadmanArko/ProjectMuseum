using Godot;
using System;

public partial class LoadingText : Label
{
	private string loadingText = "Loading";
	private int dotsCount = 0;

	[Export] private Timer _timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer.Start();
	}
	private void UpdateLoadingText()
	{
		// Update the loading text with dots
		Text = $"{loadingText}{new string('.', dotsCount)}";

		// Increase dotsCount and reset if it's more than 3
		dotsCount = (dotsCount + 1) % 4;
	}

	private void OnTimerTimeout()
	{
		// Called when the timer runs out, update the loading text
		UpdateLoadingText();
	}
	
}
