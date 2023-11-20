using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class BuilderCardSlotsController : ColorRect
{
	[Export] private PackedScene _builderCardScene;
	[Export] private GridContainer _builderCardContainer;

	private List<ExhibitVariation> _exhibitVariations;
	private HttpRequest _httpRequestForGettingExhibitVariations;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_httpRequestForGettingExhibitVariations = new HttpRequest();
		AddChild(_httpRequestForGettingExhibitVariations);
		_httpRequestForGettingExhibitVariations.RequestCompleted += HttpRequestForGettingExhibitVariationsOnRequestCompleted;

		_httpRequestForGettingExhibitVariations.Request(ApiAddress.MuseumApiPath + "GetAllExhibitVariations");
	}

	private void HttpRequestForGettingExhibitVariationsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_exhibitVariations = JsonSerializer.Deserialize<List<ExhibitVariation>>(jsonStr);
		foreach (var exhibitVariation in _exhibitVariations)
		{
			var card = _builderCardScene.Instantiate();
			card.GetNode<BuilderCard>(".").SetUpBuilderCard(exhibitVariation);
			_builderCardContainer.AddChild(card);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
