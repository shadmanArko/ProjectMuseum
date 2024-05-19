using Godot;
using System;
using System.Collections.Generic;
using ProjectMuseum.Models;

public partial class ProductCardForShop : ColorRect
{
	[Export] private OptionButton _productOptionButton;
	[Export] private LineEdit _productPrice;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_productOptionButton.Clear();
		
		_productOptionButton.ItemSelected += ProductOptionButtonOnItemSelected;
	}

	public void Initialize(Shop shop, List<Product> allProducts)
	{
		_productOptionButton.Clear();
	}
	private void ProductOptionButtonOnItemSelected(long index)
	{
		GD.Print($"Pressed Option {_productOptionButton.GetItemText(index.GetHashCode())}");
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
