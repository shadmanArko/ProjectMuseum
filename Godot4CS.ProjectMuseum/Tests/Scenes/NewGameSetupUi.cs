using Godot;
using System;

public partial class NewGameSetupUi : Control
{
	[Export] public Button StartButton;

	[Export] public LineEdit LineEdit;
	[Export] public OptionButton OptionButton;

	[Export] public CheckButton CheckButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StartButton.Pressed += StartButtonOnPressed;
	}

	private void StartButtonOnPressed()
	{
		GetTree().ChangeSceneToFile("res://Tests/Scenes/museum_drag_and_drop.tscn");
		GD.Print($"Name: {LineEdit.Text}, Gender: {OptionButton.Text}, Tutorial: { CheckButton.ButtonPressed}" );
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
