using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

public partial class BuilderCard : Control
{
	[Export] public Button textureButton;
	private ExhibitVariation _exhibitVariation;
	private BuilderCardType _builderCardType;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		textureButton.Pressed += TextureButtonOnPressed;
	}

	private void TextureButtonOnPressed()
	{
		MuseumActions.OnClickBuilderCard.Invoke(_builderCardType, _exhibitVariation.VariationName);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetUpBuilderCard(ExhibitVariation exhibitVariation)
	{
		Texture2D texture = GD.Load<Texture2D>($"res://Assets/2D/Sprites/Exhibits/{exhibitVariation.ExhibitDecoration}.png");
		textureButton.Icon = texture;
		_builderCardType = BuilderCardType.Exhibit;
		_exhibitVariation = exhibitVariation;
	}
}
