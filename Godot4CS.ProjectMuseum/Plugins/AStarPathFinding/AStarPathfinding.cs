using System;
using System.Collections.Generic;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Plugins.AStarPathFinding
{
    public class AStarPathfinding
    {
        
        //todo Connect with main program
        // public int GridSizeX = 10; // Adjust this based on your grid size
        // public int GridSizeY = 10; // Adjust this based on your grid size
        
        //private Node[,] _grid;
        private List<AStarNode> _openSet;
        private HashSet<AStarNode> _closedSet;

        private readonly int _gridSizeX;
        private readonly int _gridSizeY;
        //private readonly Node[,] _grid;

        public AStarPathfinding(int gridSizeX, int gridSizeY /*, Node[,] grid*/)
        {
            _gridSizeX = gridSizeX;
            _gridSizeY = gridSizeY;
            //_grid = grid;
        }


        public List<Vector2I> FindPath(Vector2I startTileCoordinates, Vector2I goalTileCoordinates, AStarNode[,] grid)
        {
            startTileCoordinates *= -1;
            goalTileCoordinates *= -1;
            // _grid = new Node[GridSizeX, GridSizeY];
            //
            // for (int x = 0; x < GridSizeX; x++)
            // {
            //     for (int y = 0; y < GridSizeY; y++)
            //     {
            //         var tempTileCoord = new TileCoordinates
            //         {
            //             X = x,
            //             Y = y
            //         };
            //        // _grid[x, y] = new Node(tempTileCoord, null, int.MaxValue, float.MaxValue, true); //todo change is walkable
            //     }
            // }

            _openSet = new List<AStarNode>();
            _closedSet = new HashSet<AStarNode>();

            AStarNode startNode = grid[startTileCoordinates.X, startTileCoordinates.Y];
            AStarNode goalNode = grid[goalTileCoordinates.X, goalTileCoordinates.Y];
            
            startNode.GCost = 0; // Set GCost of the start node to zero
            startNode.HCost = GetDistance(startNode, goalNode); // Set HCost using the heuristic
            
            _openSet.Add(startNode);

            while (_openSet.Count > 0)
            {
                AStarNode currentNode = GetLowestFCostNode(_openSet);

                if (currentNode == goalNode)
                {
                    // Path found
                    return RetracePath(startNode, goalNode);
                }

                _openSet.Remove(currentNode);
                _closedSet.Add(currentNode);

                foreach (AStarNode neighbor in GetNeighbors(currentNode, grid))
                {
                    if (!neighbor.IsWalkable || _closedSet.Contains(neighbor))
                        continue;

                    float newGCost = currentNode.GCost + GetDistance(currentNode, neighbor);

                    if (newGCost < neighbor.GCost || !_openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newGCost;
                        neighbor.HCost = GetDistance(neighbor, goalNode);
                        neighbor.Parent = currentNode;

                        if (!_openSet.Contains(neighbor))
                            _openSet.Add(neighbor);
                    }
                }
            }

            // No path found
            // Debug.Log("No path found.");
            return null;
        }
        private AStarNode GetLowestFCostNode(List<AStarNode> nodeList)
        {
            AStarNode lowestFCostAStarNode = nodeList[0];

            for (int i = 1; i < nodeList.Count; i++)
            {
                if (nodeList[i].FCost < lowestFCostAStarNode.FCost)
                    lowestFCostAStarNode = nodeList[i];
            }

            return lowestFCostAStarNode;
        }
        
        private List<AStarNode> GetNeighbors(AStarNode aStarNode, AStarNode[,] grid)
        {
            List<AStarNode> neighbors = new List<AStarNode>();

            int[] neighborOffsets = { -1, 0, 1 };

            foreach (int xOffset in neighborOffsets)
            {
                foreach (int yOffset in neighborOffsets)
                {
                    // Skip the center (current) node and include diagonal neighbors
                    if (xOffset != 0 || yOffset != 0)
                    {
                        int neighborX = aStarNode.TileCoordinateX + xOffset;
                        int neighborY = aStarNode.TileCoordinateY + yOffset;

                        if (neighborX >= 0 && neighborX < _gridSizeX && neighborY >= 0 && neighborY < _gridSizeY)
                        {
                            AStarNode neighbor = grid[neighborX, neighborY];
                            // Adjust the movement cost for diagonal neighbors to be higher
                            int movementCost = xOffset != 0 && yOffset != 0 ? 14 : 10; // 14 for diagonals, 10 for straight
                            neighbor.GCost = aStarNode.GCost + movementCost;
                            neighbors.Add(neighbor);
                        }
                    }
                }
            }

            return neighbors;
        }

        
        private List<Vector2I> RetracePath(AStarNode startAStarNode, AStarNode endAStarNode)
        {
            List<Vector2I> path = new List<Vector2I>();
            AStarNode currentAStarNode = endAStarNode;

            while (currentAStarNode != startAStarNode)
            {
                path.Add(new Vector2I(currentAStarNode.TileCoordinateX * -1, currentAStarNode.TileCoordinateY * -1));// todo If it makes problem then create new TileCoord and assign it.
                currentAStarNode = currentAStarNode.Parent;
            }

            path.Reverse();
            return path ;
        }
        
        private int GetDistance(AStarNode aStarNodeA, AStarNode aStarNodeB)
        {
            int distanceX = Math.Abs(aStarNodeA.TileCoordinateX - aStarNodeB.TileCoordinateX);
            int distanceY = Math.Abs(aStarNodeA.TileCoordinateY - aStarNodeB.TileCoordinateY);

            // Adjust the cost of diagonal movement as desired (e.g., multiply by 1.4 for 45-degree movement)
            int diagonalCost = 14; // Cost of diagonal movement (approximately 1.4 times straight cost)
            int straightCost = 10; // Cost of straight movement

            int diagonalSteps = Math.Min(distanceX, distanceY);
            int straightSteps = Math.Abs(distanceX - distanceY);

            // if (diagonalSteps != 0)
            // {
            //     aStarNodeB.MovecostFromPreviousTile = 3;
            // }
            // else
            // {
            //     aStarNodeB.MovecostFromPreviousTile = 2;
            // }

            return  ((diagonalCost * diagonalSteps) + (straightCost * straightSteps));
        }


    }
    
    
}