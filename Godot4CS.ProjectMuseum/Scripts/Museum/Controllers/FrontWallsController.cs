using Godot;
using System;
using System.Threading.Tasks;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class FrontWallsController : Node2D
{
	[Export] private Array<Sprite2D> _walls;
	[Export] private Texture2D _mainWallTexture;
	[Export] private Texture2D _midWallTexture;
	[Export] private Texture2D _smallWallTexture;
	[Export] private Vector2 _mainWallOffset;
	[Export] private Vector2 _midWallOffset;
	[Export] private Vector2 _smallWallOffset;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		MuseumActions.OnClickWallHeightChangeButton += OnClickWallHeightChangeButton;
		SetWallTextureAndOffset(_mainWallTexture, _mainWallOffset);
	}

	private void OnClickWallHeightChangeButton(WallHeightEnum obj)
	{
		switch (obj)
		{
			case WallHeightEnum.Original:
				SetWallTextureAndOffset(_mainWallTexture, _mainWallOffset);
				break;
			case WallHeightEnum.Mid:
				SetWallTextureAndOffset(_midWallTexture, _midWallOffset);
				break;
			case WallHeightEnum.Small:
				SetWallTextureAndOffset(_smallWallTexture, _smallWallOffset);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
		}
	}

	void SetWallTextureAndOffset(Texture2D wallTexture, Vector2 textureOffset)
	{
		foreach (var wall in _walls)
		{
			wall.Texture = wallTexture;
			wall.Offset = textureOffset;
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnClickWallHeightChangeButton -= OnClickWallHeightChangeButton;

	}
}
