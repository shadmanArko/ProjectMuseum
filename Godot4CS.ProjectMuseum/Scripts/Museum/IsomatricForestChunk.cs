using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;
using Array = Godot.Collections.Array;

public partial class IsomatricForestChunk : Sprite2D
{
	[Export] private Button _expansionButton;
	[Export] private Vector2I _expansionOrigin;
	[Export] private Array<IsomatricForestChunk> _neighbourChunks;
	[Export] private bool _firstNeighbourOfMuseum;
	[Export] public bool _alreadyExpanded;
	private Color _startColor;

	private MuseumTileContainer _museumTileContainer;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		_startColor = Modulate;
		_expansionButton.MouseEntered += ExpansionButtonOnMouseEntered;
		_expansionButton.MouseExited += ExpansionButtonOnMouseExited;
		_expansionButton.Pressed += ExpansionButtonOnPressed;
		MuseumActions.OnMuseumExpanded += OnMuseumExpanded;
		await Task.Delay(1000);
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
	}

	private void OnMuseumExpanded()
	{
		CheckIfThisChunkIsExpanded();
	}

	private void CheckIfThisChunkIsExpanded()
	{
		foreach (var museumTile in _museumTileContainer.MuseumTiles)
		{
			if (museumTile.XPosition == _expansionOrigin.X && museumTile.YPosition == _expansionOrigin.Y)
			{
				_alreadyExpanded = true;
				return;
			}
		}

		_alreadyExpanded = false;
	}

	private void ExpansionButtonOnPressed()
	{
		MuseumActions.OnCallForMuseumExpansion?.Invoke(_expansionOrigin);
	}

	private void ExpansionButtonOnMouseExited()
	{
		Modulate = _startColor;
	}

	void CheckForExpansionEligibility()
	{
		foreach (var neighbourChunk in _neighbourChunks)
		{
			if (neighbourChunk._alreadyExpanded)
			{
				_expansionButton.Visible = true;
				return;
			}
		}

		if (_firstNeighbourOfMuseum)
		{
			_expansionButton.Visible = true;
			return;
		}

		_expansionButton.Visible = false;
	}
	private void ExpansionButtonOnMouseEntered()
	{
		Modulate = Colors.Brown;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_expansionButton.MouseEntered -= ExpansionButtonOnMouseEntered;
		_expansionButton.MouseExited -= ExpansionButtonOnMouseExited;
	}
}
