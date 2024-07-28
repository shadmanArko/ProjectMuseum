using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public class FlyingEnemyAi
{
    private Random _random = new();

    #region Rest

    private readonly int _restPlaceSearchRadius = 3;
    private readonly int _explorePlaceSearchRadius = 5;
    
    public List<Vector2I> FindRestingTiles(Vector2I enemyPos, MineGenerationVariables mineGenerationVariables)
    {
        var mine = mineGenerationVariables.Mine;
        var startingX = Mathf.Clamp(enemyPos.X - _restPlaceSearchRadius, 0, mine.GridWidth);
        var endingX = Mathf.Clamp(enemyPos.X + _restPlaceSearchRadius, 0, mine.GridWidth);
        var startingY = Mathf.Clamp(enemyPos.Y - _restPlaceSearchRadius, 0, mine.GridLength);
        var endingY = Mathf.Clamp(enemyPos.X - _restPlaceSearchRadius, 0, mine.GridLength);

        var listOfRestingTiles = new List<Vector2I>();
        for (var i = startingX; i < endingX; i++)
        {
            for (var j = startingY; j < endingY; j++)
            {
                var cell = mineGenerationVariables.GetCell(new Vector2I(i, j));
                if(cell == null) continue;
                if(!cell.IsBreakable || cell.IsBroken || !cell.IsInstantiated) continue;
                var lowerCell = mineGenerationVariables.GetCell(new Vector2I(i, j + 1));
                if(lowerCell == null) continue;
                if(!lowerCell.IsBreakable || !lowerCell.IsBroken || !lowerCell.IsInstantiated) continue;
                listOfRestingTiles.Add(new Vector2I(lowerCell.PositionX, lowerCell.PositionY));
            }
        }

        return listOfRestingTiles;
    }

    #endregion

    #region Explore

    public List<Vector2I> FindExploringPosition(Vector2I enemyPos, MineGenerationVariables mineGenerationVariables)
    {
        var mine = mineGenerationVariables.Mine;
        var startingX = Mathf.Clamp(enemyPos.X - _explorePlaceSearchRadius, 0, mine.GridWidth);
        var endingX = Mathf.Clamp(enemyPos.X + _explorePlaceSearchRadius, 0, mine.GridWidth);
        var startingY = Mathf.Clamp(enemyPos.Y - _explorePlaceSearchRadius, 0, mine.GridLength);
        var endingY = Mathf.Clamp(enemyPos.X - _explorePlaceSearchRadius, 0, mine.GridLength);
        
        var possibleExploreTiles = new List<Vector2I>();
        for (var i = startingX; i < endingX; i++)
        {
            for (var j = startingY; j < endingY; j++)
            {
                var cell = mineGenerationVariables.GetCell(new Vector2I(i, j));
                if(cell == null) continue;
                if (!cell.IsBroken || !cell.IsBreakable || !cell.IsInstantiated || !cell.IsRevealed) continue;
                possibleExploreTiles.Add(new Vector2I(cell.PositionX, cell.PositionY));
            }
        }

        return possibleExploreTiles;
    }

    #endregion

    #region Utilities

    private Cell GetCellByPos(Vector2I cellPos, MineGenerationVariables mineGenerationVariables)
    {
        var cell = mineGenerationVariables.GetCell(cellPos);
        return cell;
    }

    #endregion
}