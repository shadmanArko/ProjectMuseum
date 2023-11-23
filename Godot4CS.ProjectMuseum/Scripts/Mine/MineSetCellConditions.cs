using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public static class MineSetCellConditions
{
    public static void SetTileMapCell(Vector2I tilePos, Cell cell, MineGenerationView mineGenerationView)
    {
        //var tilePos = mineGenerationView.LocalToMap(pos);

        if (!cell.IsInstantiated)
        {
            SetBlankCell(mineGenerationView, tilePos);
        }
        else
        {
            if (!cell.IsBreakable)
            {
                SetUnbreakableCell(mineGenerationView, tilePos);
            }
            else
            {
                if (cell.IsRevealed)
                {
                    SetBreakableCell(mineGenerationView, cell, tilePos);
                }
                else
                {
                    SetUnrevealedCell(mineGenerationView, tilePos);
                }
            }
        }
    }

    #region Set Cell Methods

    private static void SetUnrevealedCell(MineGenerationView mineGenerationView, Vector2I tilePos)
    {
        mineGenerationView.SetCell(0, tilePos,mineGenerationView.TileSourceId, new Vector2I(2,3));
    }

    private static void SetBlankCell(MineGenerationView mineGenerationView, Vector2I tilePos)
    {
        mineGenerationView.SetCell(0, tilePos,mineGenerationView.TileSourceId, new Vector2I(5,2));
    }

    private static void SetUnbreakableCell(MineGenerationView mineGenerationView, Vector2I tilePos)
    {
        mineGenerationView.SetCell(0, tilePos,mineGenerationView.TileSourceId, new Vector2I(2,3));
    }

    private static void SetBreakableCell(MineGenerationView mineGenerationView, Cell cell, Vector2I tilePos)
    {
        var n = 0;
        if (cell.TopBrokenSide)
        {
            n += 1;
        }
        if (cell.RightBrokenSide)
        {
            n += 2;
        }
        if (cell.BottomBrokenSide)
        {
            n += 4;
        }
        if (cell.LeftBrokenSide)
        {
            n += 8;
        }
        mineGenerationView.SetCell(0, tilePos,mineGenerationView.TileSourceId, TileAtlasCoords(n));
    }

    #endregion

    public static void SetCrackOnTiles(Vector2I pos, Vector2I mouseDir, Cell cell, MineGenerationView mineGenerationView)
    {
        var tilePos = mineGenerationView.LocalToMap(pos);
        if (cell.HitPoint >= 3)
        {
            SetCrackBasedOnDigDirection(mineGenerationView, mouseDir, tilePos, new Vector2I(0,0));
        }
        else if (cell.HitPoint >= 2)
        {
            SetCrackBasedOnDigDirection(mineGenerationView, mouseDir, tilePos, new Vector2I(1,0));
        }
        else if (cell.HitPoint >= 1)
        {
            SetCrackBasedOnDigDirection(mineGenerationView, mouseDir, tilePos, new Vector2I(2,0));
        }
        else
        {
            mineGenerationView.EraseCell(1,tilePos);
            mineGenerationView.SetCell(0,tilePos,mineGenerationView.TileSourceId,new Vector2I(5,2));
        }
    }
    
    private static void SetCrackBasedOnDigDirection(MineGenerationView mineGenerationView,Vector2I mouseDir, Vector2I tilePos, Vector2I coords)
    {
        switch (mouseDir)
        {
            case Vector2I(1,0):
                mineGenerationView.SetCell(1,tilePos,mineGenerationView.TileCrackSourceId,coords,1);
                break;
            case Vector2I(-1,0):
                mineGenerationView.SetCell(1,tilePos,mineGenerationView.TileCrackSourceId,coords);
                break;
            case Vector2I(0,-1):
                mineGenerationView.SetCell(1,tilePos,mineGenerationView.TileCrackSourceId,coords,2);
                break;
            case Vector2I(0,1):
                mineGenerationView.SetCell(1,tilePos,mineGenerationView.TileCrackSourceId,coords,3);
                break;
        }
    }

    private static Vector2I TileAtlasCoords(int tileValue)
    {
        return tileValue switch
        {
            0 => new Vector2I(1, 1),
            1 => new Vector2I(1, 0),
            2 => new Vector2I(2, 1),
            3 => new Vector2I(2, 0),
            4 => new Vector2I(1, 2),
            5 => new Vector2I(4, 0),
            6 => new Vector2I(2, 2),
            7 => new Vector2I(5, 0),
            8 => new Vector2I(0, 1),
            9 => new Vector2I(0, 0),
            10 => new Vector2I(3, 2),
            11 => new Vector2I(3, 1),
            12 => new Vector2I(0, 2),
            13 => new Vector2I(3, 0),
            14 => new Vector2I(3, 3),
            _ => new Vector2I(6, 0)
        };
    }
}