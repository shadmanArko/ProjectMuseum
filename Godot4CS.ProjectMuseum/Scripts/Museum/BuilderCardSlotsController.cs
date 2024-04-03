using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class BuilderCardSlotsController : Control
{
	[Export] private PackedScene _builderCardScene;
	[Export] private Button _buildersPanelClosingButton;
	[Export] private GridContainer _builderCardContainer;

	private List<ExhibitVariation> _exhibitVariations = new List<ExhibitVariation>();
	private List<DecorationShopVariation> _decorationShopVariations = new List<DecorationShopVariation>();
	private List<DecorationOtherVariation> _decorationOtherVariations = new List<DecorationOtherVariation>();
	private List<SanitationVariation> _sanitationVariations = new List<SanitationVariation>();
	private List<TileVariation> _tileVariations = new List<TileVariation>();
	private List<WallpaperVariation> _wallpaperVariations = new List<WallpaperVariation>();
	private HttpRequest _httpRequestForGettingExhibitVariations;
	private HttpRequest _httpRequestForGettingDecorationShopVariations;
	private HttpRequest _httpRequestForGettingDecorationOtherVariations;
	private HttpRequest _httpRequestForGettingTileVariations;
	private HttpRequest _httpRequestForGettingWallpaperVariations;
	private HttpRequest _httpRequestForGettingSanitationVariations;
	// Called when the node enters the scene tree for the first time.
	private BuilderCardType _builderCardType;
	public override void _Ready()
	{
		_httpRequestForGettingExhibitVariations = new HttpRequest();
		_httpRequestForGettingDecorationShopVariations = new HttpRequest();
		_httpRequestForGettingDecorationOtherVariations = new HttpRequest();
		_httpRequestForGettingTileVariations = new HttpRequest();
		_httpRequestForGettingWallpaperVariations = new HttpRequest();
		_httpRequestForGettingSanitationVariations = new HttpRequest();
		AddChild(_httpRequestForGettingExhibitVariations);
		AddChild(_httpRequestForGettingDecorationShopVariations);
		AddChild(_httpRequestForGettingDecorationOtherVariations);
		AddChild(_httpRequestForGettingTileVariations);
		AddChild(_httpRequestForGettingWallpaperVariations);
		AddChild(_httpRequestForGettingSanitationVariations);
		_httpRequestForGettingExhibitVariations.RequestCompleted += HttpRequestForGettingExhibitVariationsOnRequestCompleted;
		_httpRequestForGettingDecorationShopVariations.RequestCompleted += HttpRequestForGettingDecorationShopVariationsOnRequestCompleted;
		_httpRequestForGettingDecorationOtherVariations.RequestCompleted += HttpRequestForGettingDecorationOtherVariationsOnRequestCompleted;
		_httpRequestForGettingTileVariations.RequestCompleted += HttpRequestForGettingTileVariationsOnRequestCompleted;
		_httpRequestForGettingWallpaperVariations.RequestCompleted += HttpRequestForGettingWallpaperVariationsOnRequestCompleted;
		_httpRequestForGettingSanitationVariations.RequestCompleted += HttpRequestForGettingSanitationVariationsOnRequestCompleted;
		_httpRequestForGettingExhibitVariations.Request(ApiAddress.MuseumApiPath + "GetAllExhibitVariations");
		_httpRequestForGettingDecorationShopVariations.Request(ApiAddress.MuseumApiPath + "GetAllDecorationShopVariations");
		_httpRequestForGettingDecorationOtherVariations.Request(ApiAddress.MuseumApiPath + "GetAllDecorationOtherVariations");
		_httpRequestForGettingTileVariations.Request(ApiAddress.MuseumApiPath + "GetAllTileVariations");
		_httpRequestForGettingWallpaperVariations.Request(ApiAddress.MuseumApiPath + "GetAllWallpaperVariations");
		_httpRequestForGettingSanitationVariations.Request(ApiAddress.MuseumApiPath + "GetAllSanitationVariations");
		if (_buildersPanelClosingButton != null)
		{
			_buildersPanelClosingButton.Pressed += BuildersPanelClosingButtonOnPressed;
		}

		MuseumActions.OnBottomPanelBuilderCardToggleClicked += ReInitialize;
	}

	private void HttpRequestForGettingSanitationVariationsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_sanitationVariations = JsonSerializer.Deserialize<List<SanitationVariation>>(jsonStr);
		GD.Print($"Got sanitation variations {_sanitationVariations.Count}");
	}

	private void BuildersPanelClosingButtonOnPressed()
	{
		Visible = false;
	}

	private void HttpRequestForGettingDecorationOtherVariationsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_decorationOtherVariations = JsonSerializer.Deserialize<List<DecorationOtherVariation>>(jsonStr);
	}

	private void HttpRequestForGettingDecorationShopVariationsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_decorationShopVariations = JsonSerializer.Deserialize<List<DecorationShopVariation>>(jsonStr);
	}

	private void HttpRequestForGettingWallpaperVariationsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_wallpaperVariations = JsonSerializer.Deserialize<List<WallpaperVariation>>(jsonStr);
	}

	private void HttpRequestForGettingTileVariationsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_tileVariations = JsonSerializer.Deserialize<List<TileVariation>>(jsonStr);
	}


	private void ReInitialize(BuilderCardType builderCardType)
	{
		ClearCardsContainer();
		_builderCardType = builderCardType;
		switch (builderCardType)
		{
			case BuilderCardType.Exhibit:
				ShowExhibitsCards();
				break;
			case BuilderCardType.DecorationShop:
				ShowDecorationCards();
				break;
			case BuilderCardType.DecorationOther:
				ShowOtherDecorationCards();
				break;
			case BuilderCardType.Flooring:
				ShowFlooringCards();
				break;
			case BuilderCardType.Wallpaper:
				ShowWallpaperCards();
				break;
			case BuilderCardType.Sanitation:
				ShowSanitationCards();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(builderCardType), builderCardType, null);
		}
		
	}

	private void ShowSanitationCards()
	{
		GD.Print("Came to show sanitation cards");
		foreach (var sanitationVariation in _sanitationVariations)
		{
			GD.Print($"Came to show  cards {sanitationVariation.SanitationId}");

			var card = _builderCardScene.Instantiate();
			card.GetNode<BuilderCard>(".").SetUpBuilderCard(_builderCardType, sanitationVariation.SanitationId, 4);
			_builderCardContainer.AddChild(card);
		}
		
	}

	private void ShowOtherDecorationCards()
	{
		if (_decorationOtherVariations.Count <= 0) return;
		
		foreach (var decorationOtherVariation in _decorationOtherVariations)
		{
			var card = _builderCardScene.Instantiate();
			GD.Print($"other dec {decorationOtherVariation.NumberOfFrames}");
			card.GetNode<BuilderCard>(".").SetUpBuilderCard(_builderCardType, decorationOtherVariation.VariationName, decorationOtherVariation.NumberOfFrames);
			_builderCardContainer.AddChild(card);
		}
	}

	private void ShowWallpaperCards()
	{
		if (_wallpaperVariations.Count <= 0) return;
		
		foreach (var wallpaperVariation in _wallpaperVariations)
		{
			var card = _builderCardScene.Instantiate();
			card.GetNode<BuilderCard>(".").SetUpBuilderCard(_builderCardType, wallpaperVariation.VariationName, wallpaperVariation.NumberOfFrames);
			_builderCardContainer.AddChild(card);
		}
	}

	private void ShowFlooringCards()
	{
		if (_tileVariations.Count <= 0) return;
		
		foreach (var tileVariation in _tileVariations)
		{
			var card = _builderCardScene.Instantiate();
			card.GetNode<BuilderCard>(".").SetUpBuilderCard(_builderCardType, tileVariation.VariationName, tileVariation.NumberOfFrames);
			_builderCardContainer.AddChild(card);
		}
	}

	private void ClearCardsContainer()
	{
		foreach (var child in _builderCardContainer.GetChildren())
		{
			child.QueueFree();
		}
	}

	private void ShowDecorationCards()
	{
		if (_decorationShopVariations.Count <= 0) return;
		
		foreach (var decorationShopVariation in _decorationShopVariations)
		{
			var card = _builderCardScene.Instantiate();
			card.GetNode<BuilderCard>(".").SetUpBuilderCard(_builderCardType, decorationShopVariation.VariationName, decorationShopVariation.NumberOfFrames);
			_builderCardContainer.AddChild(card);
		}
	}

	private void HttpRequestForGettingExhibitVariationsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_exhibitVariations = JsonSerializer.Deserialize<List<ExhibitVariation>>(jsonStr);
	}

	private void ShowExhibitsCards()
	{
		if (_exhibitVariations.Count <= 0) return;
		
		foreach (var exhibitVariation in _exhibitVariations)
		{
			var card = _builderCardScene.Instantiate();
			card.GetNode<BuilderCard>(".").SetUpBuilderCard(_builderCardType, exhibitVariation.VariationName, exhibitVariation.NumberOfFrames);
			_builderCardContainer.AddChild(card);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForGettingExhibitVariations.RequestCompleted -= HttpRequestForGettingExhibitVariationsOnRequestCompleted;
		_httpRequestForGettingDecorationShopVariations.RequestCompleted -= HttpRequestForGettingDecorationShopVariationsOnRequestCompleted;
		_httpRequestForGettingDecorationOtherVariations.RequestCompleted -= HttpRequestForGettingDecorationOtherVariationsOnRequestCompleted;
		_httpRequestForGettingTileVariations.RequestCompleted -= HttpRequestForGettingTileVariationsOnRequestCompleted;
		_httpRequestForGettingWallpaperVariations.RequestCompleted -= HttpRequestForGettingWallpaperVariationsOnRequestCompleted;
		_httpRequestForGettingSanitationVariations.RequestCompleted -= HttpRequestForGettingSanitationVariationsOnRequestCompleted;
		MuseumActions.OnBottomPanelBuilderCardToggleClicked -= ReInitialize;
		if (_buildersPanelClosingButton != null)
		{
			_buildersPanelClosingButton.Pressed -= BuildersPanelClosingButtonOnPressed;
		}


	}
}
