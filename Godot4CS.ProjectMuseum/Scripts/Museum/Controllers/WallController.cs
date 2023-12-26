using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class WallController : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnClickBuilderCard += OnClickBuilderCard;
	}

	private void OnClickBuilderCard(BuilderCardType builderCardType, string cardName)
	{
		if (builderCardType == BuilderCardType.Wallpaper)
		{
			Texture2D texture2D = GD.Load<Texture2D>($"res://Assets/2D/Sprites/{builderCardType}s/{cardName}.png");
			MuseumActions.OnPreviewWallpaperUpdated?.Invoke(texture2D);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
