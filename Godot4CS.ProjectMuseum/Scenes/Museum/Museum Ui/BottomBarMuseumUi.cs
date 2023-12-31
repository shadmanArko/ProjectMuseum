using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class BottomBarMuseumUi : Control
{
	[Export] private Button _newExhibitButton;
	[Export] private Button _decorationsButton;
	[Export] private Button _flooringButton;
	[Export] private Button _wallpapersButton;
	[Export] private Button _exhibitButton;
	[Export] private Label _museumMoneyTextField;
	[Export] private Label _museumGuestNumberTextField;
	[Export] private Control _builderCardPanel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_newExhibitButton.Pressed += NewExhibitButtonOnPressed;
		_decorationsButton.Pressed += DecorationsButtonOnPressed;
		_flooringButton.Pressed += FlooringButtonOnPressed;
		_exhibitButton.Pressed += DisableBuilderCard;
		_wallpapersButton.Pressed += WallpapersButtonOnPressed;
		MuseumActions.OnMuseumBalanceUpdated += OnMuseumBalanceUpdated;
		MuseumActions.TotalGuestsUpdated += TotalGuestsUpdated;
	}

	private void WallpapersButtonOnPressed()
	{
		MuseumActions.OnBottomPanelBuilderCardToggleClicked?.Invoke(BuilderCardType.Wallpaper);
		EnableBuilderCard();
	}

	private void FlooringButtonOnPressed()
	{
		MuseumActions.OnBottomPanelBuilderCardToggleClicked?.Invoke(BuilderCardType.Flooring);
		EnableBuilderCard();
	}

	private void NewExhibitButtonOnPressed()
	{
		MuseumActions.OnBottomPanelBuilderCardToggleClicked?.Invoke(BuilderCardType.Exhibit);
		EnableBuilderCard();
	}

	private void DecorationsButtonOnPressed()
	{
		MuseumActions.OnBottomPanelBuilderCardToggleClicked?.Invoke(BuilderCardType.Decoration);
		EnableBuilderCard();
	}

	private void TotalGuestsUpdated(int totalNumber)
	{
		_museumGuestNumberTextField.Text = totalNumber.ToString();
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
		MuseumActions.TotalGuestsUpdated -= TotalGuestsUpdated;
		MuseumActions.OnMuseumBalanceUpdated -= OnMuseumBalanceUpdated;
	}
}
