using Godot;
using System;

public partial class BottomBarMuseumUi : Control
{
	[Export] private Button _newExhibitButton;
	[Export] private Button _exhibitButton;

	[Export] private Control _builderCardPanel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_newExhibitButton.Pressed += EnableBuilderCard;
		_exhibitButton.Pressed += DisableBuilderCard;
	}

	private void EnableBuilderCard()
	{
		_builderCardPanel.Visible = true;
	}
	private void DisableBuilderCard()
	{
		_builderCardPanel.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
