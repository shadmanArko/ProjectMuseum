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

        var cells = new List<Cell>();
        cells.Add(cell);
        cells.Add(ChangeTopNeighbourBottomBrokenSide(tilePos.X, tilePos.Y, mineGenerationVariables.Mine));
        cells.Add(ChangeRightNeighbourLeftBrokenSide(tilePos.X, tilePos.Y, mineGenerationVariables.Mine));
        cells.Add(ChangeBottomNeighbourTopBrokenSide(tilePos.X, tilePos.Y, mineGenerationVariables.Mine));
        cells.Add(ChangeLeftNeighbourRightBrokenSide(tilePos.X, tilePos.Y, mineGenerationVariables.Mine));
        return cells;
    }
    
    private static Cell ChangeTopNeighbourBottomBrokenSide(int positionX, int positionY, global::ProjectMuseum.Models.Mine mine)
    {
        var cell = mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX + 0) && cell1.PositionY == (positionY + 1));
        if (cell != null)
        {
            cell.BottomBrokenSide = true;
            cell.IsRevealed = true;
        }
        
        return cell;
    }
    
    private static Cell ChangeRightNeighbourLeftBrokenSide(int positionX, int positionY, global::ProjectMuseum.Models.Mine mine)
    {
        var cell = mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX + 1) && cell1.PositionY == (positionY + 0));
        if (cell != null)
        {
            cell.LeftBrokenSide = true;
            cell.IsRevealed = true;
        }
        return cell;
    }
    
    private static Cell ChangeBottomNeighbourTopBrokenSide(int positionX, int positionY, global::ProjectMuseum.Models.Mine mine)
    {
        var cell = mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX + 0) && cell1.PositionY == (positionY - 1));
        if (cell != null)
        {
            cell.TopBrokenSide = true;
            cell.IsRevealed = true;
        }
        return cell;
    }
    
    private static Cell ChangeLeftNeighbourRightBrokenSide(int positionX, int positionY, global::ProjectMuseum.Models.Mine mine)
    {
        var cell = mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX - 1) && cell1.PositionY == (positionY + 0));
        if (cell != null)
        {
            cell.RightBrokenSide = true;
            cell.IsRevealed = true;
        }
        return cell;
    }
}


