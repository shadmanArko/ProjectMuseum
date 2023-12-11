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

	public void SetUpBuilderCard(BuilderCardType builderCardType, string cardName)
	{
		Texture2D texture = GD.Load<Texture2D>($"res://Assets/2D/Sprites/{builderCardType}s/{cardName}.png");
		textureButton.Icon = texture;
		_builderCardType = builderCardType;
		_cardName = cardName;
	}
}
