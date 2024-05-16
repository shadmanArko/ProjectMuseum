using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

public partial class ShopUi : Control
{
	[Export] private Button _closeButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnClickShopItem += OnClickShopItem;
		_closeButton.Pressed += CloseButtonOnPressed;
	}

	private void CloseButtonOnPressed()
	{
		Visible = false;
	}

	private void OnClickShopItem(Item item, Shop shop)
	{
		GD.Print("ShopUiOpened");
		Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnClickShopItem -= OnClickShopItem;

	}
}
