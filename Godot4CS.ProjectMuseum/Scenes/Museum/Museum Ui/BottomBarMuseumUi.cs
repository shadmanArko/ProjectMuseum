using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class BottomBarMuseumUi : Control
{
	[Export] private Button _newExhibitButton;
	[Export] private Button _exhibitButton;
	[Export] private Label _museumMoneyTextField;
	[Export] private Control _builderCardPanel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_newExhibitButton.Pressed += EnableBuilderCard;
		_exhibitButton.Pressed += DisableBuilderCard;
		MuseumActions.OnMuseumBalanceUpdated += OnMuseumBalanceUpdated;
	}

	private void OnMuseumBalanceUpdated(float amount)
	{
		_museumMoneyTextField.Text = amount.ToString("0.00");
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

	public override void _ExitTree()
	{
		_newExhibitButton.Pressed -= EnableBuilderCard;
		_exhibitButton.Pressed -= DisableBuilderCard;
		MuseumActions.OnMuseumBalanceUpdated -= OnMuseumBalanceUpdated;
	}
}
