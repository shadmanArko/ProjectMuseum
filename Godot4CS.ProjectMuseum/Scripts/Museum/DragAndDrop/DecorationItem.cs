using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Newtonsoft.Json;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

public partial class DecorationItem : Item
{
	private string _variationName;
	private HttpRequest _httpRequestForPlacingDecorationItem;
	private BuilderCardType _builderCardType;
	private Shop _shopData;
	private List<Product> _allShopProducts;
	private MuseumTileContainer _museumTileContainer;
	public int numberOfProductsSold = 0;
	public float totalRevenue = 0;
	public override void _Ready()
	{
		base._Ready();
		_httpRequestForPlacingDecorationItem = new HttpRequest();
		AddChild(_httpRequestForPlacingDecorationItem);
		_httpRequestForPlacingDecorationItem.RequestCompleted += HttpRequestForPlacingDecorationItemOnRequestCompleted;
		MuseumActions.OnProductPriceUpdated += OnProductPriceUpdated;
		MuseumActions.OnProductReplaced += OnProductReplaced;
		MuseumActions.OnGettingAllProducts += OnGettingAllProducts;
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
	}

	private void OnGettingAllProducts(List<Product> obj)
	{
		_allShopProducts = obj;
	}

	private void OnProductReplaced(Product currentProduct, string newProductName, float price)
	{
		// GD.PrintErr($"Came to replace product cpid {currentProduct.ShopId}, csid {_shopData.Id}");
		if (_shopData != null && currentProduct.ShopId == _shopData.Id)
		{
			
			foreach (var product in _shopData.CoreShopFunctional.DefaultProducts.ToList())
			{
				if (product.Id == currentProduct.Id)
				{
					_shopData.CoreShopFunctional.DefaultProducts.Remove(product);
					GD.Print("Removing product");
				}
			}
			foreach (var product in _museumTileContainer.Products)
			{
				if (product.ProductVariant == newProductName)
				{
					product.BasePrice = price;
					product.Id = Guid.NewGuid().ToString();
					product.ShopId = _shopData.Id;
					_shopData.CoreShopFunctional.DefaultProducts.Add(product);
					
				}
			}
			MuseumActions.OnClickShopItem?.Invoke(this, _shopData);
		}
	}

	private void OnProductPriceUpdated(Product currentProduct, float price)
	{
		if (_shopData != null && currentProduct.ShopId == _shopData.Id)
		{
			foreach (var product in _shopData.CoreShopFunctional.DefaultProducts)
			{
				if (product.Id == currentProduct.Id)
				{
					product.BasePrice = price;
					GD.Print($"Updating product price to {product.BasePrice}");
				}
			}
		}
	}

	private void HttpRequestForPlacingDecorationItemOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		GD.Print("Http1 result " + jsonStr);
		if (_builderCardType == BuilderCardType.DecorationShop)
		{
			var tilesWithShopsDto = JsonSerializer.Deserialize<TilesWithShopsDTO>(jsonStr);
			_museumTileContainer.MuseumTiles = tilesWithShopsDto.MuseumTiles;
			_museumTileContainer.Shops = tilesWithShopsDto.DecorationShops!;
			_shopData = tilesWithShopsDto.Shop;
		}else if (_builderCardType == BuilderCardType.DecorationOther)
		{
			var tiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
			_museumTileContainer.MuseumTiles = tiles;
		}
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!selectedItem) return;
		Vector2I mouseTile = GameManager.tileMap.LocalToMap(GetGlobalMousePosition());
        
		// Check if the tile is eligible for this item placement
		if (_lastCheckedTile != mouseTile)
		{
			Vector2 localPos = GameManager.tileMap.MapToLocal(mouseTile);
			Vector2 worldPos = GameManager.tileMap.ToGlobal(localPos);
			_eligibleForItemPlacementInTile = CheckIfTheTileIsEligible(mouseTile);
			// Modulate = _eligibleForItemPlacementInTile ? _eligibleColor : _ineligibleColor;
			SetMaterialBasedOnEligibility();
			// GD.Print($"{eligibleForItemPlacementInTile}");
			// Apply effect based on eligibility
			GlobalPosition = worldPos;
			_lastCheckedTile = mouseTile;
			MuseumActions.OnItemUpdated?.Invoke();
		}
		if (selectedItem && Input.IsActionPressed("ui_left_click"))
		{
			if (!_eligibleForItemPlacementInTile)
			{
				//GD.Print("Not Eligible tile");
				return;
			}

			HandleItemPlacement();
			// OnItemPlaced?.Invoke(ItemPrice);
			selectedItem = false;
			Modulate = _originalColor;
			SetMaterialWithoutBlend();
		}
		if (selectedItem && Input.IsActionPressed("ui_right_click"))
		{
			QueueFree();
		}
	}

	public void Initialize(string cardName, BuilderCardType builderCardType)
	{
		selectedItem = true;
		_builderCardType = builderCardType;
		_itemType = _builderCardType == BuilderCardType.DecorationShop ? ItemTypes.Shop : ItemTypes.Decoration;
		_variationName = cardName;
		MakeObjectsFloating();
	}
	public void ReInitializeShop(Shop shop)
	{
		_shopData = shop;
		_itemType = ItemTypes.Shop;
	}
	private new void HandleItemPlacement()
	{
		GD.Print("Handled item placement from Decoration Item");
		List<string> tileIds = new List<string>();
		foreach (var matchingExhibitPlacementConditionData in _listOfMatchingExhibitPlacementConditionDatas)
		{
			tileIds.Add(GetTileId(new Vector2I(matchingExhibitPlacementConditionData.TileXPosition, matchingExhibitPlacementConditionData.TileYPosition)));
		}
		string[] headers = { "Content-Type: application/json"};
		var body = JsonConvert.SerializeObject(tileIds);
		string url = "";
		
		if (_builderCardType == BuilderCardType.DecorationShop)
		{
			url = $"{ApiAddress.MuseumApiPath}PlaceAShopOnTiles/{tileIds[0]}/{_variationName}/{Frame}";

		}else if (_builderCardType == BuilderCardType.DecorationOther)
		{
			url = $"{ApiAddress.MuseumApiPath}PlaceOtherDecorationOnTiles/{tileIds[0]}/{_variationName}/{Frame}";
		}
		_httpRequestForPlacingDecorationItem.Request(url, headers, HttpClient.Method.Get, body);
		GD.Print("Handling exhibit placement");
		MuseumActions.OnMuseumBalanceReduced?.Invoke(ItemPrice);
		MuseumActions.OnItemUpdated?.Invoke();
		OnItemPlacedOnTile(GlobalPosition);
		DisableItemPlacementShadow();
	}
	// public override void _Input(InputEvent @event)
	// {
		
	// 	}
	// 	
	// }
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (Input.IsActionJustReleased("ui_right_click"))
		{
			if (GetRect().HasPoint(GetLocalMousePosition()))
			{
				if (_itemType == ItemTypes.Shop)
				{
					MuseumActions.OnClickShopItem?.Invoke(this, _shopData);
				}

			}
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForPlacingDecorationItem.RequestCompleted -= HttpRequestForPlacingDecorationItemOnRequestCompleted;
		MuseumActions.OnProductPriceUpdated -= OnProductPriceUpdated;
		MuseumActions.OnProductReplaced -= OnProductReplaced;
		MuseumActions.OnGettingAllProducts -= OnGettingAllProducts;

	}
}
