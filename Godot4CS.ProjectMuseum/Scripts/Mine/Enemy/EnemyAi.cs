using System;
using System.Collections.Generic;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public class EnemyAi
{
    private Random _rand = new();
    
    #region Loitering Range

    private const int LeftLoiteringRange = -3;
    private const int RightLoiteringRange = 3;

    #endregion

    #region Dig In and Dig Out Range

    private const int InitialPosRange = -5;
    private const int FinalPosRange = 5;

    #endregion
    
    /// <summary>
    /// returns null if loitering path cannot be determined else
    /// returns starting and ending position position for loitering
    /// </summary>
    /// <param name="currentPos"></param>
    /// <returns></returns>
    public Tuple<Vector2, Vector2> DetermineLoiteringPath(Vector2 currentPos)
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
            if (bottomCell.IsBroken || !bottomCell.IsInstantiated)
            {
                if (cell.PositionX < currentTilePos.X)
                {
                    cellsToLoiter.Clear();
                    continue;
                }
                
                if(cell.PositionX > currentTilePos.X)
                    break;
            }
            
            var cellPos = new Vector2I(cell.PositionX, cell.PositionY);
            if(cellPos == currentTilePos) continue;
            if(cellsToLoiter.Contains(cell)) continue;
            cellsToLoiter.Add(cell);
        }

        GD.Print($"loiter cell count: {cellsToLoiter.Count}");
        if (cellsToLoiter.Count <= 0) return null;

        var rightMostPos = new Vector2(100000,10000);
        var leftMostPos = new Vector2(0,0);
        foreach (var cell in cellsToLoiter)
        {
            if (cell.PositionX < rightMostPos.X)
                rightMostPos = new Vector2(cell.PositionX, cell.PositionY) * cellSize;
            if(cell.PositionX > leftMostPos.X)
                leftMostPos = new Vector2(cell.PositionX, cell.PositionY) * cellSize;
        }

        GD.Print($"Loiter positions left:{leftMostPos}, right:{rightMostPos}");
        leftMostPos += new Vector2(cellSize / 2f, 0);
        rightMostPos += new Vector2(-cellSize / 2f, 0);
        GD.Print($"left cell: ({cellsToLoiter[0].PositionX},{cellsToLoiter[0].PositionY})  right cell: ({cellsToLoiter[^1].PositionX},{cellsToLoiter[^1].PositionY})");
        return new Tuple<Vector2, Vector2>(leftMostPos, rightMostPos);
    }

    public Vector2 DetermineDigOutPosition(Vector2I currentMapPos)
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
                    continue;
                
                if (cell.IsInstantiated && cell.IsBreakable && cell.IsBroken && cell.IsRevealed && !cell.HasCellPlaceable)
                {
                    if(cell.PositionX == playerPosInMap.X && cell.PositionY == playerPosInMap.Y) continue;
                    var bottomCell = mineGenerationVariables.GetCell(new Vector2I(i, j+1));
                    if (bottomCell == null) continue;
                    
                    if ((!bottomCell.IsBroken || !bottomCell.IsBreakable) && bottomCell.IsRevealed && bottomCell.IsInstantiated)
                    {
                        if (!cellsToSpawn.Contains(cell))
                        {
                            var cellPos = new Vector2I(cell.PositionX, cell.PositionY);
                            if(cellPos == currentMapPos) continue;
                            cellsToSpawn.Add(cell);
                        }
                    }
                }
            }
        }

        GD.Print($"Cell count is: {cellsToSpawn.Count}");
        
        var randomCell = cellsToSpawn[_rand.Next(0, cellsToSpawn.Count)];
        var offset = new Vector2(cellSize / 2f, cellSize * (3f/ 4f));
        var targetPos =
            new Vector2(randomCell.PositionX, randomCell.PositionY) * mineGenerationVariables.Mine.CellSize + offset;
        return targetPos;
    }

    /// <summary>
    /// returns a vector if valid path and zero vector for invalid path
    /// </summary>
    /// <param name="currentPos"></param>
    /// <returns></returns>
    public Vector2 CheckForPathValidity(Vector2 currentPos)
    {
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var playerControllerVariables = ReferenceStorage.Instance.PlayerControllerVariables;
        
        var enemyCell = GetTargetCell(currentPos);
        var playerCell = GetTargetCell(playerControllerVariables.Position);
        if (enemyCell == null || playerCell == null)
            return Vector2.Zero;

        if (enemyCell.PositionY != playerCell.PositionY) return Vector2.Zero;
        int initialCellPos;
        int finalCellPos;

        if (playerCell.PositionX > enemyCell.PositionX)
        {
            initialCellPos = enemyCell.PositionX;
            finalCellPos = playerCell.PositionX;
        }
        else
        {
            initialCellPos = playerCell.PositionX;
            finalCellPos = enemyCell.PositionX;
        }
        
        for (var i = initialCellPos; i <= finalCellPos; i++)
        {
            var cell = mineGenerationVariables.GetCell(new Vector2I(i, enemyCell.PositionY));
            if(cell == null) continue;
            if(!cell.IsBroken || !cell.IsInstantiated) continue;
            if(cell.PositionX == enemyCell.PositionX && cell.PositionY == enemyCell.PositionY) continue;
            
            var bottomCell = mineGenerationVariables.GetCell(new Vector2I(i, enemyCell.PositionY+1));
            if(bottomCell == null) continue;
            if (bottomCell.IsBroken || !bottomCell.IsInstantiated) return Vector2.Zero;
        }
        // GD.Print($"current valid cell for chase: {playerCell.PositionX}, {playerCell.PositionY}");
        return new Vector2(playerCell.PositionX, playerCell.PositionY) * mineGenerationVariables.Mine.CellSize;
    }

    public bool CheckAttackEligibility(Vector2 currentPos)
    {
        var playerControllerVariables = ReferenceStorage.Instance.PlayerControllerVariables;
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var targetTilePos = mineGenerationVariables.MineGenView.LocalToMap(playerControllerVariables.Position);
        var enemyTilePos = mineGenerationVariables.MineGenView.LocalToMap(currentPos);
        if (enemyTilePos.Y != targetTilePos.Y) return false;
        var playerPosX = targetTilePos.X;
        return enemyTilePos.X == playerPosX || enemyTilePos.X == playerPosX - 1 || enemyTilePos.X == playerPosX + 1;
    }

    #region Utilities

    private Cell GetTargetCell(Vector2 pos)
    {
        var mineGenVar = ReferenceStorage.Instance.MineGenerationVariables;
        var currentCellPos = mineGenVar.MineGenView.LocalToMap(pos);
        var cell = mineGenVar.GetCell(currentCellPos);
        return cell;
    }

    #endregion
}