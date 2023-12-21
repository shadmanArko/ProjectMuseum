using Godot;
using System;
using System.Diagnostics.SymbolStore;

public partial class LoadingPanel : Panel
{
	[Export] private Label _label;
	[Export] private int _maxNumberOfDotsInLabel = 3;
	[Export] private float _delayBetweenDots = 1f;
	private int _currentNumberOfDot = 0;
	private string _mainText = "Loading";
	private string _dot = ".";

	private double _timer;
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_timer += delta;
		if (_timer > _delayBetweenDots)
		{
			_timer = 0;
			_currentNumberOfDot = (_currentNumberOfDot + 1) % (_maxNumberOfDotsInLabel + 1);
			_label.Text = _mainText;
			for (int i = 0; i < _currentNumberOfDot; i++)
			{
				_label.Text += _dot;
			}
		}
	}
}
