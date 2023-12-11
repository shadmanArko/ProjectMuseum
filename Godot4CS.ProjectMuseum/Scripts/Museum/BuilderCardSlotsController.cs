using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class BuilderCardSlotsController : ColorRect
{
	[Export] private PackedScene _builderCardScene;
	[Export] private GridContainer _builderCardContainer;

	private List<ExhibitVariation> _exhibitVariations = new List<ExhibitVariation>();
	private HttpRequest _httpRequestForGettingExhibitVariations;
	// Called when the node enters the scene tree for the first time.
	private BuilderCardType _builderCardType;
	public override void _Ready()
	{
		_httpRequestForGettingExhibitVariations = new HttpRequest();
		AddChild(_httpRequestForGettingExhibitVariations);
		_httpRequestForGettingExhibitVariations.RequestCompleted += HttpRequestForGettingExhibitVariationsOnRequestCompleted;
		_httpRequestForGettingExhibitVariations.Request(ApiAddress.MuseumApiPath + "GetAllExhibitVariations");

		MuseumActions.OnBottomPanelBuilderCardToggleClicked += ReInitialize;
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
			case BuilderCardType.Decoration:
				ShowDecorationCards();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(builderCardType), builderCardType, null);
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
		var card = _builderCardScene.Instantiate();
		card.GetNode<BuilderCard>(".").SetUpBuilderCard(_builderCardType, "vendingmachine1");
		_builderCardContainer.AddChild(card);
		var card2 = _builderCardScene.Instantiate();
		card2.GetNode<BuilderCard>(".").SetUpBuilderCard(_builderCardType, "vendingmachine2");
		_builderCardContainer.AddChild(card2);
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
			card.GetNode<BuilderCard>(".").SetUpBuilderCard(_builderCardType, exhibitVariation.VariationName);
			_builderCardContainer.AddChild(card);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
