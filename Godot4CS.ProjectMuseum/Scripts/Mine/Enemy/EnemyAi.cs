using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public class EnemyAi
{
    private const int TileRange = 3;
    
    public Tuple<Vector2, Vector2> HorizontalMovement(Vector2 enemyPos)
    {
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var offset = mineGenerationVariables.Mine.CellSize / 4f;
        
        var currentTilePos = mineGenerationVariables.MineGenView.LocalToMap(enemyPos);
        var startingPos = new Vector2(currentTilePos.X, currentTilePos.Y) * mineGenerationVariables.Mine.CellSize;
        var endingPos = new Vector2(currentTilePos.X, currentTilePos.Y) * mineGenerationVariables.Mine.CellSize;

        var leftTilePosRangeX = Mathf.Clamp(currentTilePos.X - TileRange, 1, mineGenerationVariables.Mine.GridWidth - 1);
        var rightTilePosRangeX = Mathf.Clamp(currentTilePos.X + TileRange, 1, mineGenerationVariables.Mine.GridWidth - 1);
        var bottomTilePosRangeY = Mathf.Clamp(currentTilePos.Y + 1, 1, mineGenerationVariables.Mine.GridLength);
        
        for (var i = currentTilePos.X; i > leftTilePosRangeX; i--)
        {
            var cell = mineGenerationVariables.GetCell(new Vector2I(i, currentTilePos.Y));
            var immediateBottomCell = mineGenerationVariables.GetCell(new Vector2I(i, bottomTilePosRangeY));
            
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
            var immediateBottomCell = mineGenerationVariables.GetCell(new Vector2I(i, bottomTilePosRangeY));
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
}