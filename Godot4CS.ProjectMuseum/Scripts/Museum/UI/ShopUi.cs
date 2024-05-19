using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class ShopUi : Control
{
	[Export] private Button _closeButton;
	[Export] private PackedScene _productCardForShop;
	[Export] private Control _productCardsParent;
	[Export] private Label _shopName;

	private List<Product> _allProducts;

	private HttpRequest _httpRequestForGettingAllProducts;

	private MuseumTileContainer _museumTileContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_httpRequestForGettingAllProducts = new HttpRequest();
		AddChild(_httpRequestForGettingAllProducts);
		_httpRequestForGettingAllProducts.RequestCompleted += HttpRequestForGettingAllProductsOnRequestCompleted;
		_httpRequestForGettingAllProducts.Request(ApiAddress.MuseumApiPath + "GetAllProducts");
		MuseumActions.OnClickShopItem += OnClickShopItem;
		_closeButton.Pressed += CloseButtonOnPressed;
		
	}
	

	private void HttpRequestForGettingAllProductsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_allProducts = JsonSerializer.Deserialize<List<Product>>(jsonStr);
		GD.Print($"Got products {_allProducts.Count}");
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
		_museumTileContainer.Products = _allProducts;
		// MuseumActions.OnGettingAllProducts?.Invoke(_allProducts);
	}

	private void CloseButtonOnPressed()
	{
		Visible = false;
	}

	private void OnClickShopItem(Item item, Shop shop)
	{
		GD.Print("ShopUiOpened");
		Visible = true;
		ReInitializeShopUi(shop);
	}

	private void ReInitializeShopUi(Shop shop)
	{
		_shopName.Text = shop.CoreShopDescriptive.DisplayName;
		foreach (var child in _productCardsParent.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var defaultProduct in shop.CoreShopFunctional.DefaultProducts)
		{
			var productCard = _productCardForShop.Instantiate();
			_productCardsParent.AddChild(productCard);
			productCard.GetNode<ProductCardForShop>(".").Initialize(defaultProduct, shop, _allProducts);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnClickShopItem -= OnClickShopItem;
		_httpRequestForGettingAllProducts.RequestCompleted -= HttpRequestForGettingAllProductsOnRequestCompleted;
		_closeButton.Pressed -= CloseButtonOnPressed;
	}
}
