using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class MuseumGate : Node2D
{
	[Export] private Texture2D _midHeightGate;
	[Export] private Texture2D _smallHeightGate;
	[Export] private Texture2D _originalHeightGate;
	[Export] private Array<Sprite2D> _gateSprites;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnClickWallHeightChangeButton += OnClickWallHeightChangeButton;
	}

	private void OnClickWallHeightChangeButton(WallHeightEnum obj)
	{
		var texture = _originalHeightGate;
		switch (obj)
		{
			case WallHeightEnum.Original:
				texture = _originalHeightGate;
				break;
			case WallHeightEnum.Mid:
				texture = _midHeightGate;
				break;
			case WallHeightEnum.Small:
				texture = _smallHeightGate;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
		}

		foreach (var gateSprite in _gateSprites)
		{
			gateSprite.Texture = texture;
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnClickWallHeightChangeButton -= OnClickWallHeightChangeButton;

	}
}
