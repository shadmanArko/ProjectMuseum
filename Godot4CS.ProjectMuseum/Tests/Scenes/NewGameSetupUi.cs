using Godot;
using System;

public partial class NewGameSetupUi : Control
{
	[Export] public Button StartButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StartButton.Pressed += StartButtonOnPressed;
	}

	private void StartButtonOnPressed()
	{
		GetTree().ChangeSceneToFile("res://Tests/Scenes/museum_drag_and_drop.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
