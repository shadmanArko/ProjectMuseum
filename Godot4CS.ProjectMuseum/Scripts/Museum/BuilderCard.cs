using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

public partial class BuilderCard : Control
{
	[Export] public Button textureButton;
	private string _cardName;
	private BuilderCardType _builderCardType;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		textureButton.Pressed += TextureButtonOnPressed;
	}

	private void TextureButtonOnPressed()
	{
		MuseumActions.OnClickBuilderCard.Invoke(_builderCardType, _cardName);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void SetUpBuilderCard(BuilderCardType builderCardType, string cardName, int numberOfFrames)
	{
		Texture2D texture = GD.Load<Texture2D>($"res://Assets/2D/Sprites/{builderCardType}s/{cardName}.png");
		AtlasTexture atlasTexture = new AtlasTexture();
		atlasTexture.Atlas = texture;
		atlasTexture.Region = new Rect2(0, 0, texture.GetWidth()/numberOfFrames, texture.GetHeight());
		textureButton.Icon = atlasTexture;
		_builderCardType = builderCardType;
		_cardName = cardName;
	}
}
