using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using Resource = ProjectMuseum.Models.MIne.Resource;

namespace Godot4CS.ProjectMuseum.Service.MineServices;

public partial class ProceduralMineGenerationService : Node
{
    private MineGenerationVariables _mineGenerationVariables;
    private ProceduralMineGenerationDto _mineGenerationDto;

    #region Variables

    private int _xSize;
    private int _ySize;
    private int _cellSize; 

    #endregion
    
    public override void _EnterTree()
    {
        InitializeDiReference();
        PopulateMineGenerationData();
    }

    #region Initializers

    private void InitializeDiReference()
    {
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _mineGenerationDto = ServiceRegistry.Resolve<ProceduralMineGenerationDto>();
    }

    private void PopulateMineGenerationData()
    {
        var mineGenDataJson =
            File.ReadAllText(
                "D:/Godot Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Game Data Folder/ProceduralGenerationData/ProceduralMineGenerationData.json");
        var mineGenData = JsonSerializer.Deserialize<ProceduralMineGenerationData>(mineGenDataJson);
        _mineGenerationDto.ProceduralMineGenerationData = mineGenData;
    }

    public async Task GenerateProceduralMine()
    {
        _xSize = _mineGenerationDto.ProceduralMineGenerationData.MineSizeX;
        _ySize = _mineGenerationDto.ProceduralMineGenerationData.MineSizeY;
        _cellSize = _mineGenerationDto.ProceduralMineGenerationData.CellSize;

        var mine = GenerateMineCellData(_xSize, _ySize, _cellSize);
    }

    #endregion

    #region Generate Ordinary Mine Cells

    private int _maxHitPoint = 40;
    
    private Mine GenerateMineCellData(int xSize, int ySize, int cellSize)
    {
        var mine = new Mine
        {
            CellSize = cellSize,
            GridWidth = xSize,
            GridLength = ySize,
            Caves = new List<Cave>(),
            WallPlaceables = new List<WallPlaceable>(),
            CellPlaceables = new List<CellPlaceable>(),
            SpecialBackdropPngInformations = new List<SpecialBackdropPngInformation>(),
            Resources = new List<Resource>()
        };
        var cells = new List<Cell>();

        for (var x = 0; x < xSize; x++)
        {
            for (var y = 0; y < ySize; y++)
            {
                var cell = new Cell
                {
                    Id = Guid.NewGuid().ToString(),
                    PositionX = x,
                    PositionY = y
                };
                if (y == 0 || y == ySize-1)
                {
                    if (y == 0 && x == xSize / 2)
                    {
                        CreateBlankCell(cell);
                        cells.Add(cell);
                        continue;
                    }
                    
                    CreateUnbreakableCell(cell);
                    cells.Add(cell);
                }
                else if (x == 0 || x == xSize -1)
                {
                    CreateUnbreakableCell(cell);
                    cells.Add(cell);
                }
                else
                {
                    CreateBreakableCell(cell);
                    cells.Add(cell);
                    if (y == 1 && x == xSize / 2)
                        cell.IsRevealed = true;
                }
            }
        }

        mine.Cells = cells;
        mine.Caves = new List<Cave>();
        
        return mine;
    }

    private void CreateBlankCell(Cell cell)
    {
        cell.IsBreakable = false;
        cell.IsBroken = false;
        cell.IsInstantiated = false;
        cell.HasArtifact = false;
        cell.HasCave = false;
        cell.MaxHitPoint = 100000;
        cell.HitPoint = 100000;
    }

    private void CreateUnbreakableCell(Cell cell)
    {
        cell.IsBreakable = false;
        cell.IsBroken = false;
        cell.IsInstantiated = true;
        cell.HasArtifact = false;
        cell.HasCave = false;
        cell.MaxHitPoint = 100000;
        cell.HitPoint = 100000;
    }

    private void CreateBreakableCell(Cell cell)
    {
        cell.IsBreakable = true;
        cell.IsBroken = false;
        cell.IsInstantiated = true;
        cell.IsRevealed = false;
        cell.HasArtifact = false;
        cell.HasCave = false;
        
        cell.MaxHitPoint = _maxHitPoint;
        cell.HitPoint = _maxHitPoint;
    }


    #endregion
}