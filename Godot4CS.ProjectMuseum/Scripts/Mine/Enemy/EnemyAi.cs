using System;
using System.Collections.Generic;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public class EnemyAi
{
    private const int TileRange = 3;
    
    private Random _rand = new();
    #region Loitering Range

    private const int LeftLoiteringRange = -3;
    private const int RightLoiteringRange = 3;

    #endregion

    #region Dig In and Dig Out Range

    private const int InitialPosRange = -3;
    private const int FinalPosRange = 3;

    #endregion
    
    public Tuple<Vector2, Vector2> BackAndForthMovement(Vector2 enemyPos)
    {
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var offset = mineGenerationVariables.Mine.CellSize / 4f;
        
        var currentTilePos = mineGenerationVariables.MineGenView.LocalToMap(enemyPos);
        var startingPos = new Vector2(currentTilePos.X, currentTilePos.Y) * mineGenerationVariables.Mine.CellSize;
        var endingPos = new Vector2(currentTilePos.X, currentTilePos.Y) * mineGenerationVariables.Mine.CellSize;

        var leftTilePosRangeX = Mathf.Clamp(currentTilePos.X - TileRange, 1, mineGenerationVariables.Mine.GridWidth - 1);
        var rightTilePosRangeX = Mathf.Clamp(currentTilePos.X + TileRange, 1, mineGenerationVariables.Mine.GridWidth - 1);
        
        for (var i = currentTilePos.X; i > leftTilePosRangeX; i--)
        {
            var cell = mineGenerationVariables.GetCell(new Vector2I(i, currentTilePos.Y));
            var immediateBottomCell = mineGenerationVariables.GetCell(new Vector2I(i, currentTilePos.Y+1));
            if(cell == null) continue;
            if (cell.IsBroken && !immediateBottomCell.IsBroken)
            {
                startingPos = new Vector2(i, currentTilePos.Y) * mineGenerationVariables.Mine.CellSize + new Vector2(offset,0);
                continue;
            }
            break;
        }
        
        for (var i = currentTilePos.X; i < rightTilePosRangeX; i++)
        {
            var cell = mineGenerationVariables.GetCell(new Vector2I(i, currentTilePos.Y));
            var immediateBottomCell = mineGenerationVariables.GetCell(new Vector2I(i, currentTilePos.Y+1));
            if (cell.IsBroken && !immediateBottomCell.IsBroken)
            {
                endingPos = new Vector2(i, currentTilePos.Y) * mineGenerationVariables.Mine.CellSize - new Vector2(offset,0);
                continue;
            }
            break;
        }

        var tuple = new Tuple<Vector2, Vector2>(startingPos, endingPos);
        GD.Print($"selected positions are: {tuple.Item1} and {tuple.Item2}");
        return tuple;
    }
    
    public Tuple<Vector2, Vector2> HorizontalMovement(Vector2 currentPos)
    {
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var cellSize = mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize /2f, cellSize * (3f/4f));
        
        var currentTilePos = mineGenerationVariables.MineGenView.LocalToMap(currentPos);

        var cellsToLoiter = new List<Cell>();
        for (var i = currentTilePos.X + LeftLoiteringRange; i <= currentTilePos.X+ RightLoiteringRange; i++)
        {
            var cell = mineGenerationVariables.GetCell(new Vector2I(i, currentTilePos.Y));
            if(cell == null) continue;
            if(!cell.IsBroken || !cell.IsInstantiated || cell.HasCellPlaceable) continue;
            
            var bottomCell = mineGenerationVariables.GetCell(new Vector2I(i, currentTilePos.Y+1));
            if(bottomCell == null) continue;
            if(bottomCell.IsBroken || !bottomCell.IsInstantiated) continue;
            var cellPos = new Vector2I(cell.PositionX, cell.PositionY);
            if(cellPos == currentTilePos) continue;
            if(cellsToLoiter.Contains(cell)) continue;
            cellsToLoiter.Add(cell);
        }

        GD.Print($"loiter cell count: {cellsToLoiter.Count}");
        if (cellsToLoiter.Count <= 0) return null;

        var leftMostPos = new Vector2(100000,10000);
        var rightMostPos = new Vector2(0,0);
        foreach (var cell in cellsToLoiter)
        {
            if (cell.PositionX < leftMostPos.X)
                leftMostPos = new Vector2(cell.PositionX, cell.PositionY) * cellSize;
            if(cell.PositionX > rightMostPos.X)
                rightMostPos = new Vector2(cell.PositionX, cell.PositionY) * cellSize;
        }

        GD.Print($"Loiter positions left:{rightMostPos}, right:{leftMostPos}");
        
        // var randomCell = cellsToLoiter[_rand.Next(0, cellsToLoiter.Count)];
        return new Tuple<Vector2, Vector2>(rightMostPos, leftMostPos);
    }

    public Vector2 SlimeDigOut(Vector2I currentMapPos)
    {
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var playerControllerVariables = ReferenceStorage.Instance.PlayerControllerVariables;
        var cellSize = mineGenerationVariables.Mine.CellSize;
        
        var playerPosInMap = mineGenerationVariables.MineGenView.LocalToMap(playerControllerVariables.Position);
        var cellsToSpawn = new List<Cell>();
        for (var i = playerPosInMap.X + InitialPosRange; i <= playerPosInMap.X+FinalPosRange; i++)
        {
            for (var j = playerPosInMap.Y+InitialPosRange; j <= playerPosInMap.Y+FinalPosRange; j++)
            {
                var cell = mineGenerationVariables.GetCell(new Vector2I(i, j));
                if (cell == null)
                {
                    GD.Print("Cell is null");
                    continue;
                }
                GD.Print($"cell is not null ({cell.PositionX},{cell.PositionY})");
                GD.Print($"instantiated:{cell.IsInstantiated}, broken:{cell.IsBroken},revealed:{cell.IsRevealed}");
                if (cell.IsInstantiated && cell.IsBreakable && cell.IsBroken && cell.IsRevealed && !cell.HasCellPlaceable)
                {
                    var bottomCell = mineGenerationVariables.GetCell(new Vector2I(i, j+1));
                    if (bottomCell == null)
                    {
                        GD.Print("bottom cell is null");
                        continue;
                    }
                    GD.Print($"bottom cell is not null {bottomCell.PositionX},{bottomCell.PositionY}");
                    GD.Print($"instantiated:{bottomCell.IsInstantiated}, broken:{bottomCell.IsBroken},revealed:{bottomCell.IsRevealed}");
                    if ((!bottomCell.IsBroken || !bottomCell.IsBreakable) && bottomCell.IsRevealed && bottomCell.IsInstantiated)
                    {
                        if (!cellsToSpawn.Contains(cell))
                        {
                            var cellPos = new Vector2I(cell.PositionX, cell.PositionY);
                            if(cellPos == currentMapPos) continue;
                            if(cellPos == playerPosInMap) continue;
                            GD.Print("cell added to list");
                            cellsToSpawn.Add(cell);
                            continue;
                        }
                        GD.Print("cell not added to list");
                    }
                }
            }
        }

        GD.Print($"Cell count is: {cellsToSpawn.Count}");
        if (cellsToSpawn.Count > 0)
        {
            foreach (var cell in cellsToSpawn)
            {
                GD.Print($"cells to spawn: {cell.PositionX},{cell.PositionY}");
            }
        }
        else
        {
            return Vector2.Zero;
        }
        
        var randomCell = cellsToSpawn[_rand.Next(0, cellsToSpawn.Count)];
        var offset = new Vector2(cellSize / 2f, cellSize * (3f/ 4f));
        var targetPos =
            new Vector2(randomCell.PositionX, randomCell.PositionY) * mineGenerationVariables.Mine.CellSize + offset;
        return targetPos;
    }
}