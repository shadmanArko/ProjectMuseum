using System;
using System.Linq;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public static class MineSetCellConditions
{
    private const int NormalBackdropLayer = 0;
    private const int SpecialBackdropLayer = 1;
    private const int WallLayer = 2;
    private const int SpecialWallLayer = 3;
    private const int ResourceAndArtifactLayer = 4;
    private const int CellCrackLayer = 5;
    private const int WallPlaceableLayer = 6;
    private const int CellPlaceableLayer = 7;
    private const int TransportBlockChainLayer = 8;
    private const int UnrevealedCellLayer = 9;

    public static void SetBackdropDuringMineGeneration(MineGenerationVariables mineGenerationVariables)
    {
        var cells = mineGenerationVariables.Mine.Cells;
        var mineGenView = mineGenerationVariables.MineGenView;
        var cellSize = mineGenerationVariables.Mine.CellSize;
        
        foreach (var cell in cells)
        {
            var pos = new Vector2(cell.PositionX * cellSize, cell.PositionY * cellSize);
            var tilePos = mineGenView.LocalToMap(pos);
            mineGenView.SetCell(NormalBackdropLayer,tilePos, 0, new Vector2I(5,2));
        }

        var specialBackdrops = mineGenerationVariables.Mine.SpecialBackdropPngInformations;
        foreach (var specialBackdrop in specialBackdrops)  
        {
            var pos = new Vector2(specialBackdrop.TilePositionX * cellSize, specialBackdrop.TilePositionY * cellSize);
            GD.Print("special backdrop position:"+pos);
            var tilePos = mineGenView.LocalToMap(pos);
            mineGenView.SetCell(SpecialBackdropLayer,tilePos,specialBackdrop.SourceId, new Vector2I(0,0));
        }
    }
    
    public static void SetTileMapCell(Vector2I tilePos, Vector2I mouseDir, Cell cell, CellCrackMaterial cellCrackMaterial, MineGenerationVariables mineGenerationVariables)
    {
        var mineGenerationView = ReferenceStorage.Instance.MineGenerationVariables.MineGenView;

        if (cell.IsInstantiated && !cell.IsBroken)
        {
            if (!cell.IsBreakable)
                SetUnbreakableCell(mineGenerationView, tilePos);
            else
            {
                if (cell.IsRevealed)
                {
                    SetBreakableCell(mineGenerationVariables, cell, cellCrackMaterial, tilePos, mouseDir);
                }
                else
                {
                    SetUnrevealedCell(mineGenerationView, tilePos);
                }
            }
        }
        else if (cell.IsInstantiated && cell.IsBroken)
        {
            if (cell.HasCave)
            {
                if(!cell.IsRevealed)
                    SetUnrevealedCell(mineGenerationView, tilePos);
                else
                    SetBrokenCell(mineGenerationVariables, tilePos);
            }
        }
    }

    #region Set Cell Methods

    private static void SetUnrevealedCell(MineGenerationView mineGenerationView, Vector2I tilePos)
    {
        mineGenerationView.SetCell(UnrevealedCellLayer, tilePos,mineGenerationView.TileSourceId, new Vector2I(2,3));
    }

    private static void SetUnbreakableCell(MineGenerationView mineGenerationView, Vector2I tilePos)
    {
        EraseCellsOnAllLayers(mineGenerationView, tilePos);
        mineGenerationView.SetCell(UnrevealedCellLayer, tilePos,mineGenerationView.TileSourceId, new Vector2I(2,3));
    }

    private static void SetBrokenCell(MineGenerationVariables mineGenerationVariables, Vector2I tilePos)
    {
        var mineGenerationView = mineGenerationVariables.MineGenView;
        EraseCellsOnAllLayers(mineGenerationView,tilePos);
        //MineCellDestroyer.DestroyCellByPosition(tilePos, mineGenerationVariables);
    }

    private static void SetBreakableCell(MineGenerationVariables mineGenerationVariables, Cell cell, CellCrackMaterial cellCrackMaterial, Vector2I tilePos, Vector2I mouseDir)
    {
        var mineGenerationView = mineGenerationVariables.MineGenView;
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
        mineGenerationView.SetCell(WallLayer, tilePos,mineGenerationView.TileSourceId, TileAtlasCoords(n));
        
        if(cell.HasArtifact)
            mineGenerationView.SetCell(ResourceAndArtifactLayer,tilePos, 2, new Vector2I(0,0));
        
        if (cell.HasResource)
        {
            var random = new Random();
            var resources = mineGenerationVariables.Mine.Resources;
            var resource = resources.FirstOrDefault(tempResource => tempResource.PositionX == tilePos.X && tempResource.PositionY == tilePos.Y);
            mineGenerationView.SetCell(ResourceAndArtifactLayer,tilePos,3,new Vector2I(random.Next(0,2), resource!.Variant == "Iron" ? 0 : 1));
        }
        SetCrackOnTiles(tilePos, mouseDir, cell, cellCrackMaterial);
    }

    #endregion

    public static void SetCrackOnTiles(Vector2I tilePos, Vector2I mouseDir, Cell cell,CellCrackMaterial cellCrackMaterial)
    {
        var mineGenerationView = ReferenceStorage.Instance.MineGenerationVariables.MineGenView;
        if(cell.HitPoint > 3) return;
        
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
            EraseCellsOnAllLayers(mineGenerationView, tilePos);
    }
    
    private static void SetCrackBasedOnDigDirection(MineGenerationView mineGenerationView,Vector2I mouseDir, int tileSourceId, Vector2I tilePos, Vector2I coords)
    {
        switch (mouseDir)
        {
            case (1,0):
                mineGenerationView.SetCell(CellCrackLayer,tilePos,tileSourceId,coords,1);
                break;
            case (-1,0):
                mineGenerationView.SetCell(CellCrackLayer,tilePos,tileSourceId,coords);
                break;
            case (0,-1):
                mineGenerationView.SetCell(CellCrackLayer,tilePos,tileSourceId,coords,2);
                break;
            case (0,1):
                mineGenerationView.SetCell(CellCrackLayer,tilePos,tileSourceId,coords,3);
                break;
        }
    }

    private static void EraseCellsOnAllLayers(MineGenerationView mineGenerationView, Vector2I tilePos)
    {
        mineGenerationView.EraseCell(WallLayer, tilePos);
        mineGenerationView.EraseCell(CellCrackLayer, tilePos);
        mineGenerationView.EraseCell(ResourceAndArtifactLayer, tilePos);
        mineGenerationView.EraseCell(UnrevealedCellLayer, tilePos);
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