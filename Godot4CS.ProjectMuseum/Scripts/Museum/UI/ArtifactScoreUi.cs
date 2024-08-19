using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Managers;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class ArtifactScoreUi : Control
{
	[Export] private PackedScene _artifactScoreCard;
	[Export] private Control _scoreCardsParent;
	[Export] private Button _closingButton;

	private HttpRequest _httpRequestForGettingDisplayArtifacts;
	private HttpRequest _httpRequestForGettingArtifactScores;

	private List<Artifact> _displayArtifacts;
	private List<ArtifactScore> _artifactScores;
	private List<RawArtifactDescriptive> _rawArtifactDescriptiveDatas;
	private List<RawArtifactFunctional> _rawArtifactFunctionalDatas;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		_httpRequestForGettingDisplayArtifacts = new HttpRequest();
		_httpRequestForGettingArtifactScores = new HttpRequest();
		AddChild(_httpRequestForGettingDisplayArtifacts);
		AddChild(_httpRequestForGettingArtifactScores);
		_httpRequestForGettingDisplayArtifacts.RequestCompleted += HttpRequestForGettingDisplayArtifactsOnRequestCompleted;
		_httpRequestForGettingArtifactScores.RequestCompleted += HttpRequestForGettingArtifactScoresOnRequestCompleted;
		MuseumActions.OnRawArtifactFunctionalDataLoaded += OnRawArtifactFunctionalDataLoaded;
		MuseumActions.OnRawArtifactDescriptiveDataLoaded += OnRawArtifactDescriptiveDataLoaded;
		MuseumActions.OnClickArtifactsLensButton += OnClickArtifactsLensButton;
		_closingButton.Pressed += ClosingButtonOnPressed;
		// _httpRequestForGettingDisplayArtifacts.Request(ApiAddress.MuseumApiPath + "GetAllDisplayArtifacts");
		_displayArtifacts = MuseumReferenceManager.Instance.DisplayArtifactServices.GetAllArtifacts();

	}

	private void OnClickArtifactsLensButton()
	{
		Visible = true;
		_httpRequestForGettingArtifactScores.Request(ApiAddress.MuseumApiPath + "RefreshArtifactScoringService");
	}

	private void ClosingButtonOnPressed()
	{
		Visible = false;
	}

	private void OnRawArtifactDescriptiveDataLoaded(List<RawArtifactDescriptive> obj)
	{
		_rawArtifactDescriptiveDatas = obj;
		GD.Print("descriptives reciverd");
	}

	private void OnRawArtifactFunctionalDataLoaded(List<RawArtifactFunctional> obj)
	{
		_rawArtifactFunctionalDatas = obj;
		GD.Print("functional datas reciverd");
	}

	private void HttpRequestForGettingArtifactScoresOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_artifactScores = JsonSerializer.Deserialize<List<ArtifactScore>>(jsonStr);
		UpdateArtifactScorePanelUi();
	}

	private void UpdateArtifactScorePanelUi()
	{
		foreach (var child in _scoreCardsParent.GetChildren())
		{
			child.QueueFree();
		}
		foreach (var artifactScore in _artifactScores.OrderByDescending(score => score.Score))
		{
			var displayArtifact = _displayArtifacts.FirstOrDefault(artifact => artifact.Id == artifactScore.ArtifactId);
			var name = _rawArtifactDescriptiveDatas.FirstOrDefault(descriptive => descriptive.Id == displayArtifact.RawArtifactId)
				.ArtifactName;
			var instance = _artifactScoreCard.Instantiate();
			instance.GetNode<ArtifactScoreCard>(".").UpdateScoreText(name, artifactScore.Score, _rawArtifactFunctionalDatas.FirstOrDefault(functional => functional.Id == displayArtifact.RawArtifactId));
			_scoreCardsParent.AddChild(instance);
		}
	}

	private void HttpRequestForGettingDisplayArtifactsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_displayArtifacts = JsonSerializer.Deserialize<List<Artifact>>(jsonStr);
		
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForGettingDisplayArtifacts.RequestCompleted -= HttpRequestForGettingDisplayArtifactsOnRequestCompleted;
		_httpRequestForGettingArtifactScores.RequestCompleted -= HttpRequestForGettingArtifactScoresOnRequestCompleted;
		MuseumActions.OnRawArtifactFunctionalDataLoaded -= OnRawArtifactFunctionalDataLoaded;
		MuseumActions.OnRawArtifactDescriptiveDataLoaded -= OnRawArtifactDescriptiveDataLoaded;
		_closingButton.Pressed -= ClosingButtonOnPressed;
		MuseumActions.OnClickArtifactsLensButton -= OnClickArtifactsLensButton;

	}
}
