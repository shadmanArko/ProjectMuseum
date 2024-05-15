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
	[Export] private PackedScene _sanitationItem;
	[Export] public Node2D ItemsParent;


	private HttpRequest _httpRequestForGettingShops;
	private HttpRequest _httpRequestForGettingOthers;
	private HttpRequest _httpRequestForSanitations;

	private MuseumTileContainer _museumTileContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_httpRequestForGettingShops = new HttpRequest();
		_httpRequestForGettingOthers = new HttpRequest();
		_httpRequestForSanitations = new HttpRequest();
		
		AddChild(_httpRequestForGettingShops);
		AddChild(_httpRequestForGettingOthers);
		AddChild(_httpRequestForSanitations);
		_httpRequestForGettingShops.RequestCompleted += HttpRequestForGettingShopsOnRequestCompleted;
		_httpRequestForGettingOthers.RequestCompleted += HttpRequestForGettingOthersOnRequestCompleted;
		_httpRequestForSanitations.RequestCompleted += HttpRequestForSanitationsOnRequestCompleted;
		_httpRequestForGettingShops.Request(ApiAddress.MuseumApiPath + "GetAllShops");
		_httpRequestForGettingOthers.Request(ApiAddress.MuseumApiPath + "GetAllOtherDecorations");
		_httpRequestForSanitations.Request(ApiAddress.MuseumApiPath + "GetAllSanitations");
	}

	private void HttpRequestForSanitationsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var sanitations = JsonSerializer.Deserialize<List<Sanitation>>(jsonStr);
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
		_museumTileContainer.Sanitations = sanitations;
		SpawnSanitation(sanitations);
	}

	private void HttpRequestForGettingOthersOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var otherDecorations = JsonSerializer.Deserialize<List<DecorationOther>>(jsonStr);
		
		SpawnOtherDecorations(otherDecorations);
	}

	

	private void HttpRequestForGettingShopsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var shops = JsonSerializer.Deserialize<List<Shop>>(jsonStr);
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
		_museumTileContainer.Shops = shops;
		SpawnShops(shops);
	}
	private void SpawnOtherDecorations(List<DecorationOther> otherDecorations)
	{
		foreach (var otherDecoration in otherDecorations)
		{
			var instance = (Node)_decorationOtherItem.Instantiate();
			Texture2D texture2D = GD.Load<Texture2D>($"res://Assets/2D/Sprites/DecorationOthers/{otherDecoration.VariationName}.png");
			var sprite = instance.GetNode<Sprite2D>(".") ;
			sprite.Texture = texture2D;
			
			sprite.Frame = otherDecoration.RotationFrame;
			instance.GetNode<Node2D>(".").Position =
				GameManager.tileMap.MapToLocal(new Vector2I(otherDecoration.XPosition, otherDecoration.YPosition));
			ItemsParent.AddChild(instance);
		}
	}
	private void SpawnShops(List<Shop> shops)
	{
		foreach (var shop in shops)
		{
			var instance = (Node)_decorationShopItem.Instantiate();
			Texture2D texture2D = GD.Load<Texture2D>($"res://Assets/2D/Sprites/DecorationShops/{shop.CoreShopFunctional.Variant}.png");
			var sprite = instance.GetNode<Sprite2D>(".") ;
			sprite.Texture = texture2D;
			sprite.Frame = shop.RotationFrame;
			instance.GetNode<Node2D>(".").Position =
				GameManager.tileMap.MapToLocal(new Vector2I(shop.XPosition, shop.YPosition));
			ItemsParent.AddChild(instance);
		}
	}
	private void SpawnSanitation(List<Sanitation> sanitations)
	{
		foreach (var sanitation in sanitations)
		{
			var instance = (Node)_sanitationItem.Instantiate();
			Texture2D texture2D = GD.Load<Texture2D>($"res://Assets/2D/Sprites/Sanitations/{sanitation.SanitationVariationName}.png");
			var sprite = instance.GetNode<Sprite2D>(".") ;
			sprite.Texture = texture2D;
			sprite.Frame = sanitation.RotationFrame;
			instance.GetNode<Node2D>(".").Position =
				GameManager.tileMap.MapToLocal(new Vector2I(sanitation.XPosition, sanitation.YPosition));
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
		_httpRequestForGettingOthers.RequestCompleted -= HttpRequestForGettingOthersOnRequestCompleted;
		_httpRequestForSanitations.RequestCompleted -= HttpRequestForSanitationsOnRequestCompleted;


	}
}
