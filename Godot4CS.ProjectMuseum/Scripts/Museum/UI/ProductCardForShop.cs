using Godot;
using System;
using System.Collections.Generic;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

public partial class ProductCardForShop : ColorRect
{
	[Export] private OptionButton _productOptionButton;
	[Export] private LineEdit _productPrice;
	[Export] private Button _priceIncreaseButton;
	[Export] private Button _priceDecreaseButton;
	[Export] private float _priceChangingAmount = 0.05f;
	private Product _currentSelectedProduct;
	private float _currentProductPrice = 0f;
	private string _currentSelectedProductName = "";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_productOptionButton.Clear();
		_priceIncreaseButton.Pressed += PriceIncreaseButtonOnPressed;
		_priceDecreaseButton.Pressed += PriceDecreaseButtonOnPressed;
		_productOptionButton.ItemSelected += ProductOptionButtonOnItemSelected;
	}

	private void PriceDecreaseButtonOnPressed()
	{
		_currentProductPrice -= _priceChangingAmount;
		UpdatePriceText();
		MuseumActions.OnProductPriceUpdated?.Invoke(_currentSelectedProduct, _currentProductPrice);
	}

	private void PriceIncreaseButtonOnPressed()
	{
		_currentProductPrice += _priceChangingAmount;
		UpdatePriceText();
		MuseumActions.OnProductPriceUpdated?.Invoke(_currentSelectedProduct, _currentProductPrice);

	}

	public void Initialize(Product selectedProduct, Shop shop, List<Product> allProducts)
	{
		_productOptionButton.Clear();
		var count = 0;
		foreach (var product in allProducts)
		{
			if (shop.CoreShopFunctional.NeedsShopFullfills.Contains(product.FulfilsGuestNeed))
			{
				_productOptionButton.AddItem(product.ProductVariant);
				if (product.ProductVariant == selectedProduct.ProductVariant)
				{
					_productOptionButton.Select(count);
					_currentProductPrice = selectedProduct.BasePrice;
					_currentSelectedProductName = selectedProduct.ProductVariant;
					_currentSelectedProduct = selectedProduct;
					UpdatePriceText();
				}
				count++;
			}
			
		}
	}

	private void UpdatePriceText()
	{
		_productPrice.Text = _currentProductPrice.ToString("0.00");
	}

	private void ProductOptionButtonOnItemSelected(long index)
	{
		MuseumActions.OnProductReplaced?.Invoke(_currentSelectedProduct, _productOptionButton.GetItemText(index.GetHashCode()), _currentProductPrice);
		_currentSelectedProductName = _productOptionButton.GetItemText(index.GetHashCode());
		GD.Print($"Pressed Option {_productOptionButton.GetItemText(index.GetHashCode())}");
	}


	public override void _ExitTree()
	{
		base._ExitTree();
		_priceIncreaseButton.Pressed -= PriceIncreaseButtonOnPressed;
		_priceDecreaseButton.Pressed -= PriceDecreaseButtonOnPressed;
		_productOptionButton.ItemSelected -= ProductOptionButtonOnItemSelected;
	}
}
