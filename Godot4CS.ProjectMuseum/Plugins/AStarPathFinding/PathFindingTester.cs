using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Plugins.AStarPathFinding;

public partial class PathFindingTester : Godot.Node
{
	private AStarPathfinding _aStarPathfinding;
	private MuseumTileContainer _museumTileContainer;

	[Export] private Vector2I _startCoordinate;
	[Export] private Vector2I _targetCoordinate;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		await Task.Delay(5000);
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();

		foreach (var aStarNode in _museumTileContainer.AStarNodes)
		{
			GD.Print($"Node  {aStarNode.TileCoordinateX}, {aStarNode.TileCoordinateY} is {aStarNode.IsWalkable}");
		}
		
		_aStarPathfinding = new AStarPathfinding(_museumTileContainer.AStarNodes.GetLength(0), _museumTileContainer.AStarNodes.GetLength(1), false);
		List<Vector2I> path = _aStarPathfinding.FindPath(_startCoordinate, _targetCoordinate, _museumTileContainer.AStarNodes);
		if(path == null) GD.Print("Path not found");
        foreach (var pathCoordinate in path)
		{
			GD.Print( $"path coordinate is {pathCoordinate}");
		}
	}

	private void PopulateGrid(AStarNode[,] givenGrid)
	{
		Random random = new Random();

		int width = givenGrid.GetLength(0);
		int height = givenGrid.GetLength(1);

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				// Set random walkable status (50% chance of being walkable)
				bool isWalkable = true;

				// Set coordinate values
				Vector2I coordinates = new Vector2I(x, y);

				// Set other values to default or modify them as needed
				AStarNode aStarNode = new AStarNode(coordinates.X, coordinates.Y, null, 0f, 0f, isWalkable);

				// Assign the node to the grid
				givenGrid[x, y] = aStarNode;
			}
		}
	}
}