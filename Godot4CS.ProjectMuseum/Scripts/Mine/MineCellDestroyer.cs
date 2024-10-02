using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public static class MineCellDestroyer
{
    public static List<Cell> DestroyCellByPosition(Vector2I tilePos, MineGenerationVariables mineGenerationVariables)
    {
        var cell = mineGenerationVariables.GetCell(tilePos);
        cell.IsBroken = true;

        var cells = new List<Cell>
        {
            cell,
            ChangeTopNeighbourBottomBrokenSide(tilePos.X, tilePos.Y, mineGenerationVariables),
            ChangeRightNeighbourLeftBrokenSide(tilePos.X, tilePos.Y, mineGenerationVariables),
            ChangeBottomNeighbourTopBrokenSide(tilePos.X, tilePos.Y, mineGenerationVariables),
            ChangeLeftNeighbourRightBrokenSide(tilePos.X, tilePos.Y, mineGenerationVariables),
            ChangeTopLeftCorner(tilePos.X, tilePos.Y, mineGenerationVariables),
            ChangeTopRightCorner(tilePos.X, tilePos.Y, mineGenerationVariables),
            ChangeBottomLeftCorner(tilePos.X, tilePos.Y, mineGenerationVariables),
            ChangeBottomRightCorner(tilePos.X, tilePos.Y, mineGenerationVariables)
        };

        return cells;
    }
    
    private static Cell ChangeTopNeighbourBottomBrokenSide(int positionX, int positionY, MineGenerationVariables mineGenerationVariables)
    {
        var cell = mineGenerationVariables.GetCell(new Vector2I(positionX, positionY) + Vector2I.Up);
        if (cell != null)
        {
            cell.BottomBrokenSide = true;
            cell.IsRevealed = true;
        }
        
        return cell;
    }
    
    private static Cell ChangeRightNeighbourLeftBrokenSide(int positionX, int positionY, MineGenerationVariables mineGenerationVariables)
    {
        var cell = mineGenerationVariables.GetCell(new Vector2I(positionX, positionY) + Vector2I.Right);
        if (cell != null)
        {
            cell.LeftBrokenSide = true;
            cell.IsRevealed = true;
        }
        return cell;
    }
    
    private static Cell ChangeBottomNeighbourTopBrokenSide(int positionX, int positionY, MineGenerationVariables mineGenerationVariables)
    {
        var cell = mineGenerationVariables.GetCell(new Vector2I(positionX, positionY) + Vector2I.Down);
        if (cell != null)
        {
            cell.TopBrokenSide = true;
            cell.IsRevealed = true;
        }
        return cell;
    }
    
    private static Cell ChangeLeftNeighbourRightBrokenSide(int positionX, int positionY, MineGenerationVariables mineGenerationVariables)
    {
        var cell = mineGenerationVariables.GetCell(new Vector2I(positionX, positionY) + Vector2I.Left);
        if (cell != null)
        {
            cell.RightBrokenSide = true;
            cell.IsRevealed = true;
        }
        return cell;
    }
    
    private static Cell ChangeTopLeftCorner(int positionX, int positionY, MineGenerationVariables mineGenerationVariables)
    {
        var cell = mineGenerationVariables.GetCell(new Vector2I(positionX, positionY) + new Vector2I(-1,-1));
        if (cell != null)
        {
            cell.TopLeftBrokenCorner = true;
            cell.IsRevealed = true;
        }
        return cell;
    }
    
    private static Cell ChangeBottomLeftCorner(int positionX, int positionY, MineGenerationVariables mineGenerationVariables)
    {
        var cell = mineGenerationVariables.GetCell(new Vector2I(positionX, positionY) + new Vector2I(-1,1));
        if (cell != null)
        {
            cell.BottomLeftBrokenCorner = true;
            cell.IsRevealed = true;
        }
        return cell;
    }
    
    private static Cell ChangeTopRightCorner(int positionX, int positionY, MineGenerationVariables mineGenerationVariables)
    {
        var cell = mineGenerationVariables.GetCell(new Vector2I(positionX, positionY) + new Vector2I(1,-1));
        if (cell != null)
        {
            cell.TopRightBrokenCorner = true;
            cell.IsRevealed = true;
        }
        return cell;
    }
    
    private static Cell ChangeBottomRightCorner(int positionX, int positionY, MineGenerationVariables mineGenerationVariables)
    {
        var cell = mineGenerationVariables.GetCell(new Vector2I(positionX, positionY) + new Vector2I(1,1));
        if (cell != null)
        {
            cell.BottomRightBrokenCorner = true;
            cell.IsRevealed = true;
        }
        return cell;
    }
}


