using Godot;
using System;

public partial class ExhibitEditorUi : Control
{
	[Export] private Button _exitButton;
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_exitButton.Pressed += ExitButtonOnPressed;
	}

	private void ExitButtonOnPressed()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
