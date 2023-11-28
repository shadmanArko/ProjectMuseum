using Godot;
using System;

public partial class WarningUI : Panel
{
	[Export] private Button _okayButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_okayButton.Pressed += okayButtonOnPressed;
	}

	private void okayButtonOnPressed()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
