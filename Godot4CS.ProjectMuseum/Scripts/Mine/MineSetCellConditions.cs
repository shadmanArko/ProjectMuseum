using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public static class MineSetCellConditions
{
    public static void SetTileMapCell(Vector2I tilePos, Vector2I mouseDir, Cell cell, CellCrackMaterial cellCrackMaterial, MineGenerationView mineGenerationView)
    {
        if (!cell.IsInstantiated || cell.IsBroken)
            SetBlankCell(mineGenerationView, tilePos);
        else
        {
            if (!cell.IsBreakable)
                SetUnbreakableCell(mineGenerationView, tilePos);
            else
            {
                if (cell.IsRevealed) SetBreakableCell(mineGenerationView, cell, cellCrackMaterial, tilePos, mouseDir);
                else SetUnrevealedCell(mineGenerationView, tilePos);
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
        EraseCellsOnAllLayers(mineGenerationView, tilePos);
        mineGenerationView.SetCell(0, tilePos,mineGenerationView.TileSourceId, new Vector2I(5,2));
    }

    private static void SetUnbreakableCell(MineGenerationView mineGenerationView, Vector2I tilePos)
    {
        EraseCellsOnAllLayers(mineGenerationView, tilePos);
        mineGenerationView.SetCell(0, tilePos,mineGenerationView.TileSourceId, new Vector2I(2,3));
    }

    private static void SetBreakableCell(MineGenerationView mineGenerationView, Cell cell, CellCrackMaterial cellCrackMaterial, Vector2I tilePos, Vector2I mouseDir)
    {
        var n = 0;
        if (cell.TopBrokenSide)
            n += 1;
        if (cell.RightBrokenSide)
            n += 2;
        if (cell.BottomBrokenSide)
            n += 4;
        if (cell.LeftBrokenSide)
            n += 8;
        
        EraseCellsOnAllLayers(mineGenerationView, tilePos);
        mineGenerationView.SetCell(0, tilePos,mineGenerationView.TileSourceId, new Vector2I(5,2));
        mineGenerationView.SetCell(1, tilePos,mineGenerationView.TileSourceId, TileAtlasCoords(n));

        // if (cell.HasArtifact)
        //     SetArtifactCrackOnTiles(tilePos, mouseDir, cell, cellCrackMaterial, mineGenerationView);
        // else
        SetCrackOnTiles(tilePos, mouseDir, cell, cellCrackMaterial, mineGenerationView);
    }

    #endregion

    // public static void SetArtifactCrackOnTiles(Vector2I tilePos, Vector2I mouseDir, Cell cell, CellCrackMaterial cellCrackMaterial, MineGenerationView mineGenerationView)
    // {
    //     if(cell.HitPoint > 3) 
    //         return;
    //     if (cell.HitPoint == 3)
    //     {
    //         var coords = new Vector2I(cellCrackMaterial.SmallCrack.AtlasCoordX,
    //             cellCrackMaterial.SmallCrack.AtlasCoordY);
    //         SetCrackBasedOnDigDirection(mineGenerationView, mouseDir, cellCrackMaterial.SmallCrack.TileSourceId, tilePos, coords);
    //     }
    //     else if (cell.HitPoint >= 2)
    //     {
    //         var coords = new Vector2I(cellCrackMaterial.MediumCrack.AtlasCoordX,
    //             cellCrackMaterial.MediumCrack.AtlasCoordY);
    //         SetCrackBasedOnDigDirection(mineGenerationView, mouseDir, cellCrackMaterial.MediumCrack.TileSourceId, tilePos, coords);
    //     }
    //     else if (cell.HitPoint >= 1)
    //     {
    //         var coords = new Vector2I(cellCrackMaterial.LargeCrack.AtlasCoordX,
    //             cellCrackMaterial.LargeCrack.AtlasCoordY);
    //         SetCrackBasedOnDigDirection(mineGenerationView, mouseDir, cellCrackMaterial.LargeCrack.TileSourceId, tilePos, coords);
    //     }
    //     else if(cell.HitPoint <= 0)
    //     {
    //         EraseCellsOnAllLayers(mineGenerationView, tilePos);
    //         mineGenerationView.SetCell(1,tilePos,mineGenerationView.TileSourceId,new Vector2I(5,2));
    //     }
    // }

    public static void SetCrackOnTiles(Vector2I tilePos, Vector2I mouseDir, Cell cell,CellCrackMaterial cellCrackMaterial, MineGenerationView mineGenerationView)
    {
        if(cell.HitPoint > 3) 
            return;
        if(cell.HasArtifact)
            mineGenerationView.SetCell();
        
        if (cell.HitPoint == 3)
        {
            var coords = new Vector2I(cellCrackMaterial.SmallCrack.AtlasCoordX,
                cellCrackMaterial.SmallCrack.AtlasCoordY);
            SetCrackBasedOnDigDirection(mineGenerationView, mouseDir ,cellCrackMaterial.SmallCrack.TileSourceId, tilePos, coords);
        }
        else if (cell.HitPoint >= 2)
        {
            var coords = new Vector2I(cellCrackMaterial.MediumCrack.AtlasCoordX,
                cellCrackMaterial.MediumCrack.AtlasCoordY);
            SetCrackBasedOnDigDirection(mineGenerationView, mouseDir, cellCrackMaterial.MediumCrack.TileSourceId,tilePos, coords);
        }
        else if (cell.HitPoint >= 1)
        {
            var coords = new Vector2I(cellCrackMaterial.LargeCrack.AtlasCoordX,
                cellCrackMaterial.LargeCrack.AtlasCoordY);
            SetCrackBasedOnDigDirection(mineGenerationView, mouseDir, cellCrackMaterial.LargeCrack.TileSourceId, tilePos, coords);
        }
        else if(cell.HitPoint <= 0)
        {
            EraseCellsOnAllLayers(mineGenerationView, tilePos);
            mineGenerationView.SetCell(1,tilePos,mineGenerationView.TileSourceId,new Vector2I(5,2));
        }
    }
    
    private static void SetCrackBasedOnDigDirection(MineGenerationView mineGenerationView,Vector2I mouseDir, int tileSourceId, Vector2I tilePos, Vector2I coords)
    {
        switch (mouseDir)
        {
            case (1,0):
                mineGenerationView.SetCell(2,tilePos,tileSourceId,coords,1);
                break;
            case (-1,0):
                mineGenerationView.SetCell(2,tilePos,tileSourceId,coords);
                break;
            case (0,-1):
                mineGenerationView.SetCell(2,tilePos,tileSourceId,coords,2);
                break;
            case (0,1):
                mineGenerationView.SetCell(2,tilePos,tileSourceId,coords,3);
                break;
        }
    }

    private static void EraseCellsOnAllLayers(MineGenerationView mineGenerationView, Vector2I tilePos)
    {
        mineGenerationView.EraseCell(0, tilePos);
        mineGenerationView.EraseCell(1, tilePos);
        mineGenerationView.EraseCell(2, tilePos);
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