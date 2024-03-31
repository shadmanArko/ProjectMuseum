using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class DecorationsController : Node2D
{
	[Export] private PackedScene _decorationShopItem;
	[Export] private PackedScene _decorationOtherItem;
	[Export] public Node2D ItemsParent;


	private HttpRequest _httpRequestForGettingShops;
	private HttpRequest _httpRequestForGettingOthers;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_httpRequestForGettingShops = new HttpRequest();
		AddChild(_httpRequestForGettingShops);
		_httpRequestForGettingShops.RequestCompleted += HttpRequestForGettingShopsOnRequestCompleted;
		_httpRequestForGettingShops.Request(ApiAddress.MuseumApiPath + "GetAllShops");
	}

	private void HttpRequestForGettingShopsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var shops = JsonSerializer.Deserialize<List<DecorationShop>>(jsonStr);
		
		SpawnShops(shops);
	}

	private void SpawnShops(List<DecorationShop> shops)
	{
		foreach (var shop in shops)
		{
			var instance = (Node)_decorationShopItem.Instantiate();
			Texture2D texture2D = GD.Load<Texture2D>($"res://Assets/2D/Sprites/DecorationShops/{shop.ShopVariationName}.png");
			var sprite = instance.GetNode<Sprite2D>(".") ;
			sprite.Texture = texture2D;
			instance.GetNode<Node2D>(".").Position =
				GameManager.tileMap.MapToLocal(new Vector2I(shop.XPosition, shop.YPosition));
			ItemsParent.AddChild(instance);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForGettingShops.RequestCompleted -= HttpRequestForGettingShopsOnRequestCompleted;

	}
}
