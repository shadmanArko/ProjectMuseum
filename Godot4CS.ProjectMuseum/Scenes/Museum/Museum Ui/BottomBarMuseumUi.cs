using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class BottomBarMuseumUi : Control
{
	[Export] private Button _newExhibitButton;
	[Export] private Button _decorationsShopButton;
	[Export] private Button _decorationsOtherButton;
	[Export] private Button _flooringButton;
	[Export] private Button _wallpapersButton;
	[Export] private Button _exhibitButton;
	[Export] private Label _museumMoneyTextField;
	[Export] private Label _museumAddedMoneyAmountTextField;
	[Export] private Label _museumGuestNumberTextField;
	[Export] private Control _builderCardPanel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_newExhibitButton.Pressed += NewExhibitButtonOnPressed;
		_decorationsShopButton.Pressed += DecorationsShopButtonOnPressed;
		_decorationsOtherButton.Pressed += DecorationsOtherButtonOnPressed;
		_flooringButton.Pressed += FlooringButtonOnPressed;
		_exhibitButton.Pressed += DisableBuilderCard;
		_wallpapersButton.Pressed += WallpapersButtonOnPressed;
		MuseumActions.OnMuseumBalanceUpdated += OnMuseumBalanceUpdated;
		MuseumActions.TotalGuestsUpdated += TotalGuestsUpdated;
		MuseumActions.OnMuseumBalanceAdded += OnMuseumBalanceAdded;
	}

	private async void OnMuseumBalanceAdded(float obj)
	{
		_museumAddedMoneyAmountTextField.Text = $"+${obj:0.0}";
		_museumAddedMoneyAmountTextField.Visible = true;
		await Task.Delay(1000);
		_museumAddedMoneyAmountTextField.Visible = false;

	}

	private void DecorationsOtherButtonOnPressed()
	{
		MuseumActions.OnBottomPanelBuilderCardToggleClicked?.Invoke(BuilderCardType.DecorationOther);
		EnableBuilderCard();
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

	private void DecorationsShopButtonOnPressed()
	{
		MuseumActions.OnBottomPanelBuilderCardToggleClicked?.Invoke(BuilderCardType.DecorationShop);
		EnableBuilderCard();
	}

	private void TotalGuestsUpdated(int totalNumber)
	{
		_museumGuestNumberTextField.Text = totalNumber.ToString();
	}

	private async void OnMuseumBalanceUpdated(float amount)
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
		MuseumActions.OnMuseumBalanceAdded -= OnMuseumBalanceAdded;
		MuseumActions.TotalGuestsUpdated -= TotalGuestsUpdated;
		MuseumActions.OnMuseumBalanceUpdated -= OnMuseumBalanceUpdated;
	}
}
