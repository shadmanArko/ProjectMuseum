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
	[Export] public Vector2I expansionOrigin;
	[Export] private Array<IsomatricForestChunk> _allChunks;
	[Export] private Array<IsomatricForestChunk> _neighbourChunks;
	[Export] private bool _firstNeighbourOfMuseum;
	[Export] public bool _alreadyExpanded;
	[Export] public Sprite2D _outline;
	[Export] public Node2D _trees;
	private Color _startColor;
	private Vector2I _lastMuseumExpansionOrigin;
	private MuseumRunningDataContainer _museumRunningDataContainer;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		ExtractNeighbours();
		_startColor = _trees.Modulate;
		_expansionButton.MouseEntered += ExpansionButtonOnMouseEntered;
		_expansionButton.MouseExited += ExpansionButtonOnMouseExited;
		_expansionButton.Pressed += ExpansionButtonOnPressed;
		MuseumActions.OnMuseumExpanded += OnMuseumExpanded;
		MuseumActions.OnCallForMuseumExpansion += OnCallForMuseumExpansion;
		await Task.Delay(1000);
		_museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
		CheckIfThisChunkIsExpanded();
		CheckForExpansionEligibility();
	}

	private void ExtractNeighbours()
	{
		var neighbourChunks = new List<IsomatricForestChunk>();
		foreach (var chunk in _allChunks)
		{
			if (chunk.expansionOrigin != expansionOrigin && 
			    ((chunk.expansionOrigin.X == expansionOrigin.X && chunk.expansionOrigin.Y != expansionOrigin.Y && Math.Abs(chunk.expansionOrigin.Y - expansionOrigin.Y)<= 20) ||
			    (chunk.expansionOrigin.X != expansionOrigin.X && chunk.expansionOrigin.Y == expansionOrigin.Y &&  Math.Abs(chunk.expansionOrigin.X - expansionOrigin.X)<= 18))
			    )
			{
				neighbourChunks.Add(chunk);
			}
		}

		_neighbourChunks = new Array<IsomatricForestChunk>(neighbourChunks.ToArray());
	}

	private void OnCallForMuseumExpansion(Vector2I obj)
	{
		_lastMuseumExpansionOrigin = obj;
	}

	private void OnMuseumExpanded()
	{
		CheckForExpansionEligibilityAfterExpansion();
		
	}

	private void CheckForExpansionEligibilityAfterExpansion()
	{
		if (_alreadyExpanded)
		{
			GD.Print($"Already expanded at {expansionOrigin}");
			Visible = false;
			return;
		}
		foreach (var neighbourChunk in _neighbourChunks)
		{
			if (neighbourChunk.expansionOrigin == _lastMuseumExpansionOrigin)
			{
				_expansionButton.Visible = true;
				return;
			}
			
		}
	}
	private async Task WaitForMuseumTiles()
	{
		while (_museumRunningDataContainer.MuseumTiles == null)
		{
			GD.Print("Waiting");
			await Task.Delay(100); // Wait for 100 milliseconds before checking again
		}
	}
	private async void CheckIfThisChunkIsExpanded()
	{
		await WaitForMuseumTiles();
		foreach (var museumTile in _museumRunningDataContainer.MuseumTiles)
		{
			if (museumTile.XPosition == expansionOrigin.X && museumTile.YPosition == expansionOrigin.Y)
			{
				_alreadyExpanded = true;
				return;
			}
		}

		_alreadyExpanded = false;
	}

	private void ExpansionButtonOnPressed()
	{
		return;
		MuseumActions.OnCallForMuseumExpansion?.Invoke(expansionOrigin);
		_alreadyExpanded = true;
	}

	private void ExpansionButtonOnMouseExited()
	{
		_trees.Modulate = _startColor;
		_outline.Visible = false;
	}

	async void  CheckForExpansionEligibility()
	{

		await Task.Delay(500);
		if (_alreadyExpanded)
		{
			_expansionButton.Visible = false;
			Visible = false;
			return;
		}
		if (_firstNeighbourOfMuseum)
		{
			_expansionButton.Visible = true;
			Visible = true;
			return;
		}
		foreach (var neighbourChunk in _neighbourChunks)
		{
			if (neighbourChunk._alreadyExpanded)
			{
				_expansionButton.Visible = true;
				return;
			}
		}

		

		_expansionButton.Visible = false;
	}
	private void ExpansionButtonOnMouseEntered()
	{
		_trees.Modulate = Colors.AntiqueWhite;
		_outline.Visible = true;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_expansionButton.MouseEntered -= ExpansionButtonOnMouseEntered;
		_expansionButton.MouseExited -= ExpansionButtonOnMouseExited;
		_expansionButton.Pressed -= ExpansionButtonOnPressed;
		MuseumActions.OnMuseumExpanded -= OnMuseumExpanded;
		MuseumActions.OnCallForMuseumExpansion -= OnCallForMuseumExpansion;
	}
}
