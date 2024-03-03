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
            var requiredCave = new Cave
            {
                CellIds = new List<string>(),
                StalagmiteCellIds = new List<string>(),
                StalactiteCellIds = new List<string>()
            };
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
                
                var cellSize = mineGenerationVariables.Mine.CellSize;
                if (requiredCave.StalagmiteCellIds.Count > 0)
                {
                    foreach (var stalagmiteCellId in requiredCave.StalagmiteCellIds)
                    {
                        var stalagmiteCell = mineGenerationVariables.Mine.Cells.FirstOrDefault(tempCell =>
                            tempCell.Id == stalagmiteCellId);
                        var cellPos = new Vector2(stalagmiteCell!.PositionX, stalagmiteCell.PositionY)*cellSize;
                        var offset = new Vector2(cellSize / 2f, cellSize / 2f);
                        var scene = SceneInstantiator.InstantiateScene(
                            "res://Scenes/Mine/Sub Scenes/Props/Stalagmite.tscn",
                            mineGenerationVariables.MineGenView, cellPos + offset);
                    }
                }
                
                if (requiredCave.StalactiteCellIds.Count > 0)
                {
                    foreach (var stalactiteCellId in requiredCave.StalactiteCellIds)
                    {
                        var stalactiteCell = mineGenerationVariables.Mine.Cells.FirstOrDefault(tempCell =>
                            tempCell.Id == stalactiteCellId);
                        var cellPos = new Vector2(stalactiteCell!.PositionX, stalactiteCell.PositionY)*cellSize;
                        var offset = new Vector2(cellSize / 2f, cellSize / 2f);
                        var scene = SceneInstantiator.InstantiateScene(
                            "res://Scenes/Mine/Sub Scenes/Props/Stalactite.tscn",
                            mineGenerationVariables.MineGenView, cellPos + offset);
                    }
                }
            }
        }

        return totalCellsToReveal;
    }
}