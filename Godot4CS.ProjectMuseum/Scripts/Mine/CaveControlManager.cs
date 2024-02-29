using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public class CaveControlManager
{
    public static List<Cell> RevealCave(MineGenerationVariables mineGenerationVariables, List<Cell> cells)
    {
        var totalCellsToReveal = new List<Cell>();
        foreach (var cell in cells)
        {
            if(!cell.HasCave) continue;
            Cave requiredCave = null;
            foreach (var cave in mineGenerationVariables.Mine.Caves)
            {
                if (cave.CellIds.Contains(cell.Id))
                {
                    GD.Print("found required cave");
                    requiredCave = cave;
                }
            }

            if (requiredCave != null)
            {
                foreach (var cellId in requiredCave.CellIds)
                {
                    var caveCell = mineGenerationVariables.Mine.Cells.FirstOrDefault(temp => temp.Id == cellId);
                    if (caveCell == null) continue;
                    
                    caveCell.IsBroken = true;
                    caveCell.IsRevealed = true;
                    var revealedCaveCells = MineCellDestroyer.DestroyCellByPosition(new Vector2I(caveCell.PositionX, caveCell.PositionY),
                        mineGenerationVariables);

                    foreach (var revealedCaveCell in revealedCaveCells)
                    {
                        if(totalCellsToReveal.Contains(revealedCaveCell)) continue;
                        totalCellsToReveal.Add(revealedCaveCell);
                    }
                }
            }
        }

        return totalCellsToReveal;
    }
}