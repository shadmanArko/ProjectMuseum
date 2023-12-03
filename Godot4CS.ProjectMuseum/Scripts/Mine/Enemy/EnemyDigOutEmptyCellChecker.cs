using System.Threading.Tasks;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public class EnemyDigOutEmptyCellChecker
{
    public static Vector2 CheckForEmptyCellsAroundPlayer(Vector2 playerPos, MineGenerationVariables mineGenerationVariables)
    {
        var tempPos = mineGenerationVariables.MineGenView.LocalToMap(playerPos + Vector2.Down);
        var startingPosX = tempPos.X - 3;
        var endingPosX = tempPos.X + 3;
        
        for (int i = startingPosX; i < endingPosX; i++)
        {
            var tilePos = new Vector2I(i, tempPos.Y);
            var cell = mineGenerationVariables.GetCell(tilePos);
            if(cell == null) continue;
            if (cell.IsBroken)
            {
                var bottomAdjacentCellPos = tilePos + Vector2I.Down;
                var bottomCell = mineGenerationVariables.GetCell(bottomAdjacentCellPos);
                if(bottomCell == null) continue;
                if(bottomCell.IsBroken) continue;
                var cellSize = mineGenerationVariables.Mine.CellSize;
                return new Vector2(bottomCell.PositionX * cellSize, bottomCell.PositionY * cellSize);
            }
            else
            {
                var topAdjacentCellPos = tilePos + Vector2I.Up;
                var topCell = mineGenerationVariables.GetCell(topAdjacentCellPos);
                if(topCell == null) continue;
                if(topCell.IsBroken) continue;
                var cellSize = mineGenerationVariables.Mine.CellSize;
                return new Vector2(topCell.PositionX * cellSize, topCell.PositionY * cellSize);
            }
        }

        return Vector2.Zero;
    }
    
}