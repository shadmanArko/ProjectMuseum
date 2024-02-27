using System;
using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class Museum : Node2D
{
    [Export] private Godot.Collections.Dictionary<Vector2I, Vector2I> _outsideWalkableArea;

    private List<AStarNode> _aStarNodes;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GameManager.tileMap = GetNode<TileMap>("TileMap");
        // GameManager.outSideMuseumNodes = new AStarNode[,]
        SetupAStarNodesOutsideMuseum();
    }

    private void SetupAStarNodesOutsideMuseum()
    {
        _aStarNodes = new List<AStarNode>();
        var totalTilesInX = 0;
        var totalTilesInY = 0;
        foreach (var outsideAreaSelection in _outsideWalkableArea)
        {
            var startTile = outsideAreaSelection.Key;
            var endTile = outsideAreaSelection.Value;
           
            for (int x = Math.Min(startTile.X, endTile.X); x <= Math.Max(startTile.X, endTile.X); x++)
            {
                totalTilesInX++;
                for (int y = Math.Min(startTile.Y, endTile.Y);
                     y <= Math.Max(startTile.Y, endTile.Y);
                     y++)
                {
                    totalTilesInY++;
                    // _tileMap.ClearLayer(1);
                    var tilePos = new Vector2I(x, y);
                    AStarNode aStarNode = new AStarNode(tilePos.X * -1, tilePos.Y *-1, null, 0f, 0f, true);

                    // Assign the node to the grid
                    _aStarNodes.Add(aStarNode); 
                }
            }
            
        }

        var outSideMuseumNodes = new AStarNode[1, _aStarNodes.Count];
        int nodeCount = 0;
        for (int x = 0; x < outSideMuseumNodes.GetLength(0); x++)
        {
            for (int y = 0; y < outSideMuseumNodes.GetLength(1); y++)
            {
                outSideMuseumNodes[x, y] = _aStarNodes[nodeCount];
                nodeCount++;
            }
        }

        GameManager.outSideMuseumNodes = outSideMuseumNodes;

    }
}