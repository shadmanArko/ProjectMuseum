using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;
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

    private ProceduralMineGenerationData _proceduralMineGenerationData;
    private List<SpecialBackdropPngInformation> _specialBackdropsDatabase;

    #endregion

    public override void _EnterTree()
    {
        InitializeDatabases();
    }

    public override void _Ready()
    {
        InitializeDiReference();
    }

    #region Initializers

    private void InitializeDiReference()
    {
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _mineGenerationDto = ServiceRegistry.Resolve<ProceduralMineGenerationDto>();
    }

    private void InitializeDatabases()
    {
        var backdropsStr =
            File.ReadAllText(
                "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/bin/Release/net7.0/win-x64/Game Data Folder/SpecialBackdropData/SpecialBackdropPngInformation.json");
        _specialBackdropsDatabase = new List<SpecialBackdropPngInformation>();
        _specialBackdropsDatabase = JsonSerializer.Deserialize<List<SpecialBackdropPngInformation>>(backdropsStr);
        
        var mineGenDataJson =
            File.ReadAllText(
                "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Game Data Folder/ProceduralGenerationData/ProceduralMineGenerationData.json");
        _proceduralMineGenerationData = JsonSerializer.Deserialize<ProceduralMineGenerationData>(mineGenDataJson);
        GD.Print($"procedural mine gen data max caves {_proceduralMineGenerationData.NumberOfMaxCaves}");
    }

    public async Task<Mine> GenerateProceduralMine()
    {
        _xSize = _proceduralMineGenerationData.MineSizeX;
        _ySize = _proceduralMineGenerationData.MineSizeY;
        _cellSize = _proceduralMineGenerationData.CellSize;

        var mine = await GenerateMineCellData(_xSize, _ySize, _cellSize);
        // await GenerateBossCave(mine);
        // await GenerateMineCaves(mine);
        await GenerateSpecialBackdrops(mine);

        return mine;
    }

    #endregion

    #region Generate Ordinary Mine Cells

    private int _maxHitPoint = 40;

    private async Task<Mine> GenerateMineCellData(int xSize, int ySize, int cellSize)
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
                if (y == 0 || y == ySize - 1)
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
                else if (x == 0 || x == xSize - 1)
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
        mine.SpecialBackdropPngInformations = new List<SpecialBackdropPngInformation>();
        mine.Resources = new List<Resource>();
        mine.CellPlaceables = new List<CellPlaceable>();
        mine.WallPlaceables = new List<WallPlaceable>();
        mine.VineInformations = new List<VineInformation>();

        await Task.Delay(500);
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

    #region Generate Caves

    private async Task GenerateBossCave(Mine mine)
    {
        // var mineGenData = _mineGenerationDto.ProceduralMineGenerationData;
        // var mine = _mineGenerationVariables.Mine;

        foreach (var cell in mine.Cells)
            cell.HasCave = false;
        mine.Caves = new List<Cave>();

        var bossCaveSizeX = 12;//mineGenData.BossCaveSizeX; //TODO: change
        var bossCaveSizeY = 5;//mineGenData.BossCaveSizeY;  //TODO: change

        var yAxisBottomIndex = 64 - 2;//mineGenData.MineSizeY - 2;  //TODO: change
        var xAxisCenterIndex = 49 - 2;//mineGenData.MineSizeX / 2;  //TODO: change

        var xMin = xAxisCenterIndex - bossCaveSizeX / 2;
        var xMax = xAxisCenterIndex + bossCaveSizeX / 2;
        var yMin = yAxisBottomIndex - bossCaveSizeY;
        var yMax = yAxisBottomIndex;

        var noOfStalactites = 3; //Math.Clamp(mineGenData.StalactiteCount, 0, bossCaveSizeX);    //TODO: change
        var noOfStalagmites = 3; //Math.Clamp(mineGenData.StalagmiteCount, 0, bossCaveSizeX);    //TODO: change

        var cave = GenerateCave(xMin, xMax, yMin, yMax, noOfStalagmites, noOfStalactites, mine);
        Console.WriteLine($"Boss Cave Location Top:{yMin}, Bottom:{yMax}, Left:{xMin}, Right:{xMax}");
        await Task.Delay(500);
    }
    
    private async Task GenerateMineCaves(Mine mine)
    {
        var rand = new Random();
        var mineGenData = _mineGenerationDto.ProceduralMineGenerationData;

        var mineX = mine.GridWidth;
        var mineY = mine.GridLength;

        #region cavesToGenerate contains list of cave dimensions that has to be generated

        var maxCaves = 10;
        var noOfCaves = rand.Next(maxCaves / 2, maxCaves);//(mineGenData.NumberOfMaxCaves / 2, mineGenData.NumberOfMaxCaves);
        var cavesToGenerate = new List<Vector2>();
        for (var i = 0; i < noOfCaves; i++)
        {
            var tempX = rand.Next(4, 7);//(mineGenData.CaveMinSizeX, mineGenData.CaveMaxSizeX);
            var tempY = rand.Next(4, 5);//(mineGenData.CaveMinSizeY, mineGenData.CaveMaxSizeY);
            var caveDimension = new Vector2(tempX, tempY);
            cavesToGenerate.Add(caveDimension);
        }

        #endregion

        #region Creating Slots

        var noOfSlotsX = 3;
        var noOfSlotsY = 3;

        var caveSlotPosX = mineX / noOfSlotsX;
        var caveSlotPosY = mineY / noOfSlotsY;

        var offsetX = mineX / noOfSlotsX / 2;
        var offsetY = mineY / noOfSlotsY / 2;

        var listOfCoords = new List<Vector2>();
        for (var i = 0; i < noOfSlotsX; i++)
        {
            for (var j = 0; j < noOfSlotsY; j++)
            {
                var xPos = Math.Clamp(caveSlotPosX * i + offsetX, 1, mineX - 1);
                var yPos = Math.Clamp(caveSlotPosY * j + offsetY, 1, mineY - 2);

                var coord = new Vector2(xPos, yPos);
                if (listOfCoords.Contains(coord))
                {
                    i--;
                    continue;
                }

                listOfCoords.Add(coord);
            }
        }

        #endregion

        #region Create Cave

        foreach (var tempCave in cavesToGenerate)
        {
            var coord = listOfCoords[rand.Next(0, listOfCoords.Count)];
            var xMin = Math.Clamp((int)coord.X, 1, mineX - 1);
            var yMin = Math.Clamp((int)coord.Y, 1, mineY - 2);
            var xMax = Math.Clamp((int)(coord.X + tempCave.X), 1, mineX - 1);
            var yMax = Math.Clamp((int)(coord.Y + tempCave.Y), 1, mineY - 2);
            listOfCoords.Remove(coord);

            var stalagmites = rand.Next(2, xMax - xMin - 1);
            var stalactites = rand.Next(2, xMax - xMin - 1);

            // Console.WriteLine($"cave dim: ({tempCave.X},{tempCave.Y})");
            var cave = GenerateCave(xMin, xMax, yMin, yMax, stalagmites, stalactites, mine);
            Console.WriteLine(
                $"Cave generated xMin:{cave.LeftBound}, xMax:{cave.RightBound}. yMin:{cave.TopBound}, yMax:{cave.BottomBound}");
        }

        #endregion

        await Task.Delay(500);
    }
    
    private Cave GenerateCave(int xMin, int xMax, int yMin, int yMax, int stalagmiteCount, int stalactiteCount, Mine mine)
    {

        var cells = mine.Cells;
        var caveCellIds = new List<string>();
        var possibleStalagmiteCells = new List<Cell>();
        var possibleStalactiteCells = new List<Cell>();

        for (var i = xMin; i <= xMax; i++)
        {
            for (var j = yMin; j <= yMax; j++)
            {
                var cell = cells.FirstOrDefault(tempCell => tempCell.PositionX == i && tempCell.PositionY == j);
                if (cell == null) continue;

                cell.HasCave = true;
                cell.IsBroken = true;
                cell.TopBrokenSide = true;
                cell.BottomBrokenSide = true;
                cell.LeftBrokenSide = true;
                cell.RightBrokenSide = true;

                if (caveCellIds.Contains(cell.Id)) continue;
                caveCellIds.Add(cell.Id);
                if (cell.PositionY == yMin)
                    possibleStalactiteCells.Add(cell);
                else if (cell.PositionY == yMax)
                    possibleStalagmiteCells.Add(cell);
            }
        }

        var newCave = new Cave
        {
            Id = Guid.NewGuid().ToString(),
            LeftBound = xMin,
            RightBound = xMax,
            TopBound = yMin,
            BottomBound = yMax,
            CellIds = caveCellIds,
            StalagmiteCellIds = new List<string>(),
            StalactiteCellIds = new List<string>()
        };
        var rand = new Random();
        for (var numberOfStalagmites = stalagmiteCount; numberOfStalagmites > 0; numberOfStalagmites--)
        {
            var cell = possibleStalagmiteCells[rand.Next(0, possibleStalagmiteCells.Count)];
            if (newCave.StalagmiteCellIds.Contains(cell.Id)) continue;
            newCave.StalagmiteCellIds.Add(cell.Id);
        }

        for (var numberOfStalactites = stalactiteCount; numberOfStalactites > 0; numberOfStalactites--)
        {
            var cell = possibleStalactiteCells[rand.Next(0, possibleStalactiteCells.Count)];
            if (newCave.StalactiteCellIds.Contains(cell.Id)) continue;
            newCave.StalactiteCellIds.Add(cell.Id);
        }

        mine.Caves.Add(newCave);
        return newCave;
    }
    
    #endregion
    
    #region Generate Special Backdrops
    
    public async Task GenerateSpecialBackdrops(Mine mine)
    {
        var noOfSlotsX = 3;
        var noOfSlotsY = 4;
        var noOfBackdrops = 2;
        
        var slotUnitValueX = mine.GridLength / noOfSlotsX;
        var slotUnitValueY = mine.GridWidth / noOfSlotsY;
        var offsetX = slotUnitValueX / 2;
        var offsetY = slotUnitValueY / 2;
    
        var listOfCoords = new List<Vector2>();
    
        for (var i = 0; i < noOfSlotsX; i++)
        {
            for (int j = 0; j < noOfSlotsY; j++)
            {
                var xPos = i * slotUnitValueX + offsetX;
                var yPos = j * slotUnitValueY + offsetY;
                var coord = new Vector2(xPos, yPos);
                if (listOfCoords.Contains(coord)) continue;
                listOfCoords.Add(coord);
            }
        }

        var listOfBackdrops = _specialBackdropsDatabase.ToList();
    
        var rand = new Random();
        var listOfAddedBackdrops = new List<SpecialBackdropPngInformation>();
    
        for (var i = 0; i < noOfBackdrops; i++)
        {
            var tempBackdrop = listOfBackdrops[rand.Next(0, listOfBackdrops.Count)];
            var tempCoord = listOfCoords[rand.Next(0, listOfCoords.Count)];
    
            tempBackdrop.TilePositionX = (int)tempCoord.X;
            tempBackdrop.TilePositionY = (int)tempCoord.Y;
    
            if (listOfAddedBackdrops.Contains(tempBackdrop))
            {
                i--;
                continue;
            }
    
            listOfAddedBackdrops.Add(tempBackdrop);
            listOfBackdrops.Remove(tempBackdrop);
            listOfCoords.Remove(tempCoord);
        }
    
        Console.WriteLine($"Backdrop Position");
        foreach (var backdrop in listOfAddedBackdrops)
            Console.WriteLine($"X:{backdrop.TilePositionX}, Y:{backdrop.TilePositionY}");
    
        mine.SpecialBackdropPngInformations = listOfAddedBackdrops.ToList();
        await Task.Delay(500);
    }

    #endregion

}