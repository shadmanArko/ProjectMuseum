using Godot;
using System;
using ProjectMuseum.Models;

public partial class ArtifactScoreCard : ColorRect
{
	[Export] private Label _scoreText;
	[Export] private TextureRect _artifactTexture;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void UpdateScoreText(string artifactName, float score, RawArtifactFunctional rawArtifactFunctional)
	{
		_scoreText.Text = $"{artifactName} - {score:0.00}";
		_artifactTexture.Texture = GD.Load<Texture2D>(rawArtifactFunctional.LargeImageLocation);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
