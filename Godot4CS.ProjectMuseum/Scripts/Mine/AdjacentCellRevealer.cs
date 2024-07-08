using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public class AdjacentCellRevealer
{
    public static void RevealAdjacentCell(Vector2I cellPos, Vector2I mouseDir, MineGenerationVariables mineGenerationVariables, CellCrackMaterial cellCrackMaterial)
    {
        var cell = mineGenerationVariables.GetCell(cellPos);
        cell.HitPoint = 0;
        
        var cells = MineCellDestroyer.DestroyCellByPosition(cellPos, mineGenerationVariables);
        var caveCells = CaveControlManager.RevealCave(mineGenerationVariables, cells);

        foreach (var tempCell in cells)
        {
            MineSetCellConditions.SetTileMapCell(mouseDir, tempCell,
                cellCrackMaterial, mineGenerationVariables);
        }
            
        foreach (var tempCell in caveCells)
        {
            MineSetCellConditions.SetTileMapCell(mouseDir, tempCell,
                cellCrackMaterial, mineGenerationVariables);
        }

        mineGenerationVariables.BrokenCells++;
        MineActions.OnMineCellBroken?.Invoke(cellPos);

        var mouseDirection = mouseDir;
        var shakeDirection = mouseDirection.X != 0 ? 
            ShakeDirection.Horizontal : ShakeDirection.Vertical;
        ReferenceStorage.Instance.ScreenShakeController.ShakeScreen(ShakeIntensity.Mild, shakeDirection);
    }
}