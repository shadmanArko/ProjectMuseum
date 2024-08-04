using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;
using Resource = ProjectMuseum.Models.MIne.Resource;

namespace Godot4CS.ProjectMuseum.Service.MineServices;

public partial class ProceduralMineGenerationService : Node
{
    private ProceduralMineGenerationDto _mineGenerationDto;
    private RawArtifactDTO _rawArtifactDto;
    private MineCellCrackMaterial _mineCellCrackMaterial;
    
    private InventoryDTO _inventoryDto;

    #region Variables

    private Random _rand;

    private int _xSize;
    private int _ySize;
    private int _cellSize;

    private ProceduralMineGenerationData _proceduralMineGenerationDatabase;
    private List<RawArtifactDescriptive> _rawArtifactDescriptiveDatabase;
    private List<RawArtifactFunctional> _rawArtifactFunctionalDatabase;
    
    private List<SpecialBackdropPngInformation> _specialBackdropsDatabase;
    private List<ArtifactCondition> _artifactConditionsDatabase;
    private List<SiteArtifactChanceData> _siteArtifactChanceDatabase;
    private List<CellCrackMaterial> _cellCrackMaterialsDatabase;
    private List<ArtifactRarity> _artifactRarityDatabase;
    private ArtifactStorage _artifactStorageArtifactDatabase;
    
    private List<Resource> _resourceDatabase;

    #endregion

    public override void _EnterTree()
    {
        InitializeDatabases();
    }

    public override void _Ready()
    {
        InitializeDiReference();
        _rand = new Random();
        _rawArtifactDto.RawArtifactDescriptives = _rawArtifactDescriptiveDatabase;
        _rawArtifactDto.RawArtifactFunctionals = _rawArtifactFunctionalDatabase;
        _mineCellCrackMaterial.CellCrackMaterials = _cellCrackMaterialsDatabase;

        _inventoryDto.ArtifactStorage = SaveLoadService.Load().ArtifactStorage;
    }

    #region Initializers

    private void InitializeDiReference()
    {
        _mineGenerationDto = ServiceRegistry.Resolve<ProceduralMineGenerationDto>();
        _rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
        _mineCellCrackMaterial = ServiceRegistry.Resolve<MineCellCrackMaterial>();
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
    }

    private void InitializeDatabases()
    {
        _rawArtifactDto = new RawArtifactDTO();
        
        var rawArtifactDescriptiveJson =
            File.ReadAllText(
                "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Game Data Folder/RawArtifactData/RawArtifactDescriptiveData/RawArtifactDescriptiveDataEnglish.json");
        _rawArtifactDescriptiveDatabase =
            JsonSerializer.Deserialize<List<RawArtifactDescriptive>>(rawArtifactDescriptiveJson);
        
        var rawArtifactFunctionalJson =
            File.ReadAllText(
                "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Game Data Folder/RawArtifactData/RawArtifactFunctionalData/RawArtifactFunctionalData.json");
        _rawArtifactFunctionalDatabase =
            JsonSerializer.Deserialize<List<RawArtifactFunctional>>(rawArtifactFunctionalJson);
        
        var backdropsJson =
            File.ReadAllText(
                "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Dummy Data Folder/SpecialBackdropPngInformation.json");
        _specialBackdropsDatabase = new List<SpecialBackdropPngInformation>();
        _specialBackdropsDatabase = JsonSerializer.Deserialize<List<SpecialBackdropPngInformation>>(backdropsJson);

        _mineGenerationDto = new ProceduralMineGenerationDto();
        var mineGenDataJson =
            File.ReadAllText(
                "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Game Data Folder/ProceduralGenerationData/ProceduralMineGenerationData.json");
        _proceduralMineGenerationDatabase = JsonSerializer.Deserialize<ProceduralMineGenerationData>(mineGenDataJson);
        _mineGenerationDto.ProceduralMineGenerationData = _proceduralMineGenerationDatabase;
        GD.Print($"procedural mine gen data max caves {_proceduralMineGenerationDatabase.NumberOfMaxCaves}");

        var siteArtifactDataJson = File.ReadAllText(
            "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Game Data Folder/ProceduralGenerationData/SiteArtifactChanceData/SiteArtifactChanceFunctionalData/SiteArtifactChanceFunctionalData.json");
        _siteArtifactChanceDatabase = JsonSerializer.Deserialize < List<SiteArtifactChanceData>>(siteArtifactDataJson);
        GD.Print($"site artifact list: {_siteArtifactChanceDatabase.Count}");

        var artifactConditionJson =
            File.ReadAllText(
                "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Game Data Folder/ArtifactScore/ArtifactCondition.json");
        _artifactConditionsDatabase = JsonSerializer.Deserialize<List<ArtifactCondition>>(artifactConditionJson);
        GD.Print($"artifact conditions list: {_artifactConditionsDatabase.Count}");

        var artifactRarityJson =
            File.ReadAllText(
                "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Game Data Folder/ArtifactScore/ArtifactRarity.json");
        _artifactRarityDatabase = JsonSerializer.Deserialize<List<ArtifactRarity>>(artifactRarityJson);

        var resourceJson =
            File.ReadAllText(
                "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Game Data Folder/Resource/Resource.json");
        _resourceDatabase = JsonSerializer.Deserialize<List<Resource>>(resourceJson);

        var cellCrackMaterialJson =
            File.ReadAllText(
                "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Game Data Folder/CellCrackMaterial/CellCrackMaterial.json");
        _cellCrackMaterialsDatabase = JsonSerializer.Deserialize<List<CellCrackMaterial>>(cellCrackMaterialJson);
        
        // var artifactStorageArtifactsJson =
        //     File.ReadAllText(
        //         "Y:/GodotProjects/Office Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Dummy Data Folder/artifactStorage.json");
        // _artifactStorageArtifactDatabase =
        //     JsonSerializer.Deserialize<ArtifactStorage>(artifactStorageArtifactsJson);
    }

    public async Task<Mine> GenerateProceduralMine()
    {
        _xSize = _proceduralMineGenerationDatabase.MineSizeX;
        _ySize = _proceduralMineGenerationDatabase.MineSizeY;
        _cellSize = _proceduralMineGenerationDatabase.CellSize;

        var mine = await GenerateMineCellData(_xSize, _ySize, _cellSize);
        await GenerateBossCave(mine);
        await GenerateMineCaves(mine);
        await GenerateSpecialBackdrops(mine);
        await GenerateVines(mine);
        await GenerateArtifacts(mine);
        await GenerateResources(mine);

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
        GD.Print($"Boss Cave Location Top:{yMin}, Bottom:{yMax}, Left:{xMin}, Right:{xMax}");
        await Task.Delay(500);
    }
    
    private async Task GenerateMineCaves(Mine mine)
    {
        var mineGenData = _mineGenerationDto.ProceduralMineGenerationData;

        var mineX = mine.GridWidth;
        var mineY = mine.GridLength;

        #region cavesToGenerate contains list of cave dimensions that has to be generated

        var maxCaves = 10;
        var noOfCaves = _rand.Next(maxCaves / 2, maxCaves);//(mineGenData.NumberOfMaxCaves / 2, mineGenData.NumberOfMaxCaves);
        var cavesToGenerate = new List<Vector2>();
        for (var i = 0; i < noOfCaves; i++)
        {
            var tempX = _rand.Next(4, 7);//(mineGenData.CaveMinSizeX, mineGenData.CaveMaxSizeX);
            var tempY = _rand.Next(4, 5);//(mineGenData.CaveMinSizeY, mineGenData.CaveMaxSizeY);
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
            var coord = listOfCoords[_rand.Next(0, listOfCoords.Count)];
            var xMin = Math.Clamp((int)coord.X, 1, mineX - 1);
            var yMin = Math.Clamp((int)coord.Y, 1, mineY - 2);
            var xMax = Math.Clamp((int)(coord.X + tempCave.X), 1, mineX - 1);
            var yMax = Math.Clamp((int)(coord.Y + tempCave.Y), 1, mineY - 2);
            listOfCoords.Remove(coord);

            var stalagmites = _rand.Next(2, xMax - xMin - 1);
            var stalactites = _rand.Next(2, xMax - xMin - 1);

            // GD.Print($"cave dim: ({tempCave.X},{tempCave.Y})");
            var cave = GenerateCave(xMin, xMax, yMin, yMax, stalagmites, stalactites, mine);
            GD.Print(
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
        
        for (var numberOfStalagmites = stalagmiteCount; numberOfStalagmites > 0; numberOfStalagmites--)
        {
            var cell = possibleStalagmiteCells[_rand.Next(0, possibleStalagmiteCells.Count)];
            if (newCave.StalagmiteCellIds.Contains(cell.Id)) continue;
            newCave.StalagmiteCellIds.Add(cell.Id);
        }

        for (var numberOfStalactites = stalactiteCount; numberOfStalactites > 0; numberOfStalactites--)
        {
            var cell = possibleStalactiteCells[_rand.Next(0, possibleStalactiteCells.Count)];
            if (newCave.StalactiteCellIds.Contains(cell.Id)) continue;
            newCave.StalactiteCellIds.Add(cell.Id);
        }

        mine.Caves.Add(newCave);
        return newCave;
    }
    
    #endregion
    
    #region Generate Special Backdrops
    
    private async Task GenerateSpecialBackdrops(Mine mine)
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
        
        var listOfAddedBackdrops = new List<SpecialBackdropPngInformation>();
    
        for (var i = 0; i < noOfBackdrops; i++)
        {
            var tempBackdrop = listOfBackdrops[_rand.Next(0, listOfBackdrops.Count)];
            var tempCoord = listOfCoords[_rand.Next(0, listOfCoords.Count)];
    
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
    
        GD.Print($"Backdrop Position");
        foreach (var backdrop in listOfAddedBackdrops)
            GD.Print($"X:{backdrop.TilePositionX}, Y:{backdrop.TilePositionY}");
    
        mine.SpecialBackdropPngInformations = listOfAddedBackdrops.ToList();
        await Task.Delay(500);
    }

    #endregion

    #region Generate Vines

    private async Task GenerateVines(Mine mine)
    {
        var noOfVinesToGenerate = _rand.Next(10, 15);
        var minVeinRange = 4;
        var maxVeinRange = 8;
        
        var cells = mine.Cells;
        var cellsWithoutBackdrops = new List<Cell>();
        var cellsWithBackdrops = new List<Cell>();

        foreach (var cell in cells)
            cellsWithoutBackdrops.Add(cell);
        
        foreach (var specialBackdrop in mine.SpecialBackdropPngInformations)
        {
            var xMin = specialBackdrop.TilePositionX - specialBackdrop.SizeX / 2;
            var xMax = specialBackdrop.TilePositionX + specialBackdrop.SizeX / 2;
            var yMin = specialBackdrop.TilePositionY - specialBackdrop.SizeY / 2;
            var yMax = specialBackdrop.TilePositionY + specialBackdrop.SizeY / 2;

            for (var i = xMin; i < xMax; i++)
            {
                for (var j = yMin; j < yMax; j++)
                {
                    var cell = cells.FirstOrDefault(tempCell => tempCell.PositionX == i && tempCell.PositionY == j);
                    if(cell == null) continue;
                    cellsWithBackdrops.Add(cell);
                }
            }
        }

        foreach (var backdrop in cellsWithBackdrops)
            cellsWithoutBackdrops.Remove(backdrop);

        var cellsToRemove = new List<Cell>();
        foreach (var cell in cellsWithoutBackdrops)
        {
            if(cell.PositionX <= 3 || cell.PositionX > 45 || cell.PositionY <= 3 || cell.PositionY > 50)
                cellsToRemove.Add(cell);
        }

        foreach (var cell in cellsToRemove)
            cellsWithoutBackdrops.Remove(cell);
        
        var listOfVineInformations = new List<VineInformation>();
        for (var i = 0; i < noOfVinesToGenerate; i++)
        {
            var vineInfo = new VineInformation();
            var veinRange = _rand.Next(minVeinRange, maxVeinRange);
            var cellsWithVines = new List<string>();
            GD.Print($"cells without backdrops: {cellsWithoutBackdrops.Count}");
            var startingNode = cellsWithoutBackdrops[_rand.Next(0, cellsWithoutBackdrops.Count)];
            cellsWithVines.Add(startingNode.Id!);
            for (var j = 1; j <= veinRange; j++)
            {
                var nextNode = cellsWithoutBackdrops.FirstOrDefault(temp =>
                    temp.PositionX == startingNode.PositionX && temp.PositionY == startingNode.PositionY + j);
                if(nextNode == null) break;
                 GD.Print($"Vine Pos: {nextNode.PositionX}, {nextNode.PositionY}");
                cellsWithVines.Add(nextNode.Id!);
                cellsWithoutBackdrops.Remove(nextNode);
            }

            if (cellsWithVines.Count <= 0) continue;
            vineInfo.SourceId = Guid.NewGuid().ToString();
            vineInfo.VineCellPositions = cellsWithVines;
            listOfVineInformations.Add(vineInfo);
        }
        
        mine.VineInformations = listOfVineInformations;
        await Task.Delay(500);
    }
    
    #endregion
    
    #region Generate Artifacts

    private async Task GenerateArtifacts(Mine mine)
    {
        var rawArtifactFunctionals = _rawArtifactDto.RawArtifactFunctionals;
        
        var siteArtifactChance = GetSiteArtifactChanceDataBySite(_proceduralMineGenerationDatabase.Site);
        
        var regionalArtifacts = rawArtifactFunctionals!
            .Where(rawArtifactFunctional => rawArtifactFunctional.Region == _proceduralMineGenerationDatabase.Region).ToList();
        
        var weaponCount = (int) (siteArtifactChance.Weapon * _proceduralMineGenerationDatabase.TotalNoOfArtifacts);
        var armorCount = (int) (siteArtifactChance.Armor * _proceduralMineGenerationDatabase.TotalNoOfArtifacts);
        var clothingCount = (int) (siteArtifactChance.Clothing * _proceduralMineGenerationDatabase.TotalNoOfArtifacts);
        var economicCount = (int) (siteArtifactChance.Economic * _proceduralMineGenerationDatabase.TotalNoOfArtifacts);
        var vesselCount = (int) (siteArtifactChance.Vessel * _proceduralMineGenerationDatabase.TotalNoOfArtifacts);
        var leisureCount = (int) (siteArtifactChance.Leisure * _proceduralMineGenerationDatabase.TotalNoOfArtifacts);
        var toolCount = (int) (siteArtifactChance.Tool * _proceduralMineGenerationDatabase.TotalNoOfArtifacts);
        var ceremonialCount = (int) (siteArtifactChance.Ceremonial * _proceduralMineGenerationDatabase.TotalNoOfArtifacts);
        var legendaryCount = (int) (siteArtifactChance.Legendary * _proceduralMineGenerationDatabase.TotalNoOfArtifacts);
        
        var listOfRawArtifacts = new List<RawArtifactFunctional>();
    
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Weapon").ToList()
            .OrderBy(x => _rand.Next()).Take(weaponCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Armor").ToList()
            .OrderBy(x => _rand.Next()).Take(armorCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Clothing").ToList()
            .OrderBy(x => _rand.Next()).Take(clothingCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Economic").ToList()
            .OrderBy(x => _rand.Next()).Take(economicCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Vessel").ToList()
            .OrderBy(x => _rand.Next()).Take(vesselCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Leisure").ToList()
            .OrderBy(x => _rand.Next()).Take(leisureCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Tool").ToList()
            .OrderBy(x => _rand.Next()).Take(toolCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Ceremonial").ToList()
            .OrderBy(x => _rand.Next()).Take(ceremonialCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Legendary").ToList()
            .OrderBy(x => _rand.Next()).Take(legendaryCount));
        
        GD.Print($"raw artifact functional: {rawArtifactFunctionals.Count}");
        GD.Print($"list of raw artifacts: {listOfRawArtifacts.Count}");
    
        #region Adding duplicate artifacts in the list of artifacts
    
        if (listOfRawArtifacts.Count < _proceduralMineGenerationDatabase.TotalNoOfArtifacts)
        {
            var duplicateArtifacts = new List<RawArtifactFunctional>();
            var duplicateCounter = _proceduralMineGenerationDatabase.TotalNoOfArtifacts - listOfRawArtifacts.Count;
            
            for (var i = 0; i < duplicateCounter; i++)
            {
                var rawArtifact = listOfRawArtifacts[_rand.Next(0, listOfRawArtifacts.Count)];
                duplicateArtifacts.Add(rawArtifact);
            }
            
            listOfRawArtifacts.AddRange(duplicateArtifacts);
        }
    
        #endregion
    
        #region Adding Condition and Rarity to the generated artifacts
    
        var listOfArtifactRarityConditions = GetConditionRarityCombination(_proceduralMineGenerationDatabase.TotalNoOfArtifacts);
        var rarityConditionCounter = 0;
        var listOfArtifacts = new List<Artifact>();
        
        foreach (var artifactFunctional in listOfRawArtifacts)
        {
            var rarityCondition = listOfArtifactRarityConditions[rarityConditionCounter];
            var artifact = new Artifact
            {
                Id = Guid.NewGuid().ToString(),
                RawArtifactId = artifactFunctional.Id,
                Condition = rarityCondition.Item1.Condition,
                Rarity = rarityCondition.Item2.Rarity
            };
    
            rarityConditionCounter++;
            listOfArtifacts.Add(artifact);
            GD.Print($"{rarityCondition.Item1.Condition}, {rarityCondition.Item2.Rarity}");
        }
    
        #endregion
        
        var cells = mine.Cells.ToList();
        var cellsToRemove = new List<Cell>();
        foreach (var cell in cells)
        {
            if (cell.IsBroken || cell.HasCave || !cell.IsInstantiated || !cell.IsBreakable) 
                cellsToRemove.Add(cell);
        }
    
        #region Tutorial Tiles
    
        var midPoint = _proceduralMineGenerationDatabase.MineSizeX / 2;
        for (var i = midPoint -1; i < midPoint +1; i++)
        {
            for (var j = 1; j < 3; j++)
            {
                if(i == midPoint && j == 2) continue;
                cellsToRemove.Add(cells.FirstOrDefault(tempCell=> tempCell.PositionX == i && tempCell.PositionY == j));
            }
        }
    
        #endregion
        
        foreach (var cell in cellsToRemove)
        {
            cells.Remove(cell);
        }
    
        foreach (var artifact in listOfArtifacts)
        {
            var cell = cells[_rand.Next(0, cells.Count)];
            artifact.PositionX = cell.PositionX;
            artifact.PositionY = cell.PositionY;
            cells.Remove(cell);
        }
    
        var tutorialCell = cells.FirstOrDefault(tempCell => tempCell is { PositionX: 24, PositionY: 2 });
        if (tutorialCell is { HasArtifact: false })
        {
            var tutorialArtifact = new Artifact
            {
                Id = "tutorialArtifact",
                RawArtifactId = "ClassicalNativeAmericanTomahawk",
                PositionX = 24,
                PositionY = 2,
                Condition = "Decrepit",
                Rarity = "Common",
                Slot = 0
            };
                
            listOfArtifacts.Add(tutorialArtifact);
        }
        
        _rawArtifactDto.Artifacts = listOfArtifacts.ToList();
        AssignArtifactsToMine(listOfArtifacts, mine);
        GD.Print("ARTIFACTS IN DTO");
        foreach (var artifact in _rawArtifactDto.Artifacts)
        {
            GD.Print($"DTO Artifacts: {artifact.Id} === {artifact.PositionX}, {artifact.PositionY}");
        }
        
        GD.Print("ARTIFACTS IN MINE");
        foreach (var mineCell in mine.Cells)
        {
            if(!mineCell.HasArtifact) continue;
            GD.Print($"Artifact: {mineCell.ArtifactId} ||| {mineCell.PositionX}, {mineCell.PositionY}");
        }

        await Task.Delay(500);
    }

    private List<Tuple<ArtifactCondition, ArtifactRarity>> GetConditionRarityCombination(int artifactCount)
    {
        var artifactConditions = _artifactConditionsDatabase;
        var artifactRarities = _artifactRarityDatabase;
        var conditionsRarityList = new List<Tuple<ArtifactCondition, ArtifactRarity>>();
        
        GD.Print($"artifact conditions count {artifactConditions.Count}");

        foreach (var condition in artifactConditions)
            GD.Print($"conditions is {condition.Condition}");
        
        foreach (var rarity in artifactRarities)
            GD.Print($"conditions is {rarity.Rarity}");

        for (var i = 0; i < artifactCount; i++)
        {
            var conditionValue = _rand.Next(0, 101);
            var rarityValue = _rand.Next(0, 101);

            var condition = conditionValue switch
            {
                <=75 => artifactConditions[0],
                > 75 and <= 98 => artifactConditions[1],
                > 98 => artifactConditions[2]
            };

            var rarity = rarityValue switch
            {
                <= 80 => artifactRarities[0],
                > 80 and <= 99 => artifactRarities[1],
                > 99 => artifactRarities[2]
            };
            
            var tuple = new Tuple<ArtifactCondition, ArtifactRarity>(condition, rarity);
            conditionsRarityList.Add(tuple);
        }

        return conditionsRarityList;
    }

    private SiteArtifactChanceData GetSiteArtifactChanceDataBySite(string site)
    {
        var siteChanceData = _siteArtifactChanceDatabase.FirstOrDefault(temp => temp.Site == site);
        if (siteChanceData == null)
        {
            GD.PrintErr($"Fatal Error: Site does not match the database");
            return null;
        }

        GD.Print($"site chance data: legendary- {siteChanceData.Legendary}");
        return siteChanceData;
    }

    private void AssignArtifactsToMine(List<Artifact> artifacts, Mine mine)
    {
        foreach (var artifact in artifacts)
        {
            var cell = mine.Cells.FirstOrDefault(tempCell =>
                tempCell.PositionX == artifact.PositionX && tempCell.PositionY == artifact.PositionY);
            if (cell == null)
            {
                GD.PrintErr("Fatal Error: artifact position does not match any of the cell positions");
                continue;
            }
                
            var rawArtifactFunctional =
                _rawArtifactDto.RawArtifactFunctionals.FirstOrDefault(temp => temp.Id == artifact.RawArtifactId);
            if (rawArtifactFunctional == null)
            {
                GD.PrintErr("Fatal Error: Artifact rawArtifactId does not match any RawArtifactFunctionalId");
                continue;
            }
                
            cell.HasArtifact = true;
            cell.ArtifactId = artifact.Id;
            var mat = rawArtifactFunctional.Materials[0];
            cell.ArtifactMaterial = mat;
        }
    }

    #endregion

    #region Generate Resources
 
    private async Task GenerateResources(Mine mine)
    {
        var variants = new List<string> { "Coal", "Iron", "PinkQuartz", "BlueQuartz", "SmokyQuartz", "MilkyQuartz" };
        
        var cells = new List<Cell>();
        foreach (var mineCell in mine.Cells)
        {
            mineCell.HasResource = false;
            if(mineCell.HasCave || mineCell.HasArtifact || !mineCell.IsInstantiated || mineCell.IsBroken || !mineCell.IsBreakable) continue;
            cells.Add(mineCell);
        }
        mine.Resources.Clear();
        
        var numberOfRootNodes = _rand.Next(30,40);
    
        for (var i = 0; i < numberOfRootNodes; i++)
        {
            var resourceCells = new List<Cell>();
            var cell = GetRandomCell(cells);
            resourceCells.Add(cell);
            cells.Remove(cell);
            
            var rootNodeVariant = variants[_rand.Next(0, variants.Count)];
            AddResourceToMine(rootNodeVariant, cell.PositionX, cell.PositionY, mine);
    
            var resourceBranches = rootNodeVariant switch
            {
                "Iron" => _rand.Next(3, 5),
                "Coal" => _rand.Next(5, 8),
                "PinkQuartz" => _rand.Next(0, 3),
                "BlueQuartz" => _rand.Next(0, 3),
                "SmokyQuartz" => _rand.Next(0, 3),
                "MilkyQuartz" => _rand.Next(0, 3),
                _ => _rand.Next(5, 8)
            };
            var currentBranchCell = cell;
            for (var j = 0; j <= resourceBranches; j++)
            {
                currentBranchCell = GetRandomAdjacentCell(cells, currentBranchCell);
                if(resourceCells.Contains(currentBranchCell)) continue;
                resourceCells.Add(currentBranchCell);
            }
            
            foreach (var resourceCell in resourceCells)
            {
                var resource = AddResourceToMine(rootNodeVariant, resourceCell.PositionX, resourceCell.PositionY, mine);
                GD.Print($"resource {resource.Variant} {resource.PositionX},{resource.PositionY}");
                mine.Resources.Add(resource);
                FormResourceDistanceOfFourTiles(cells, resourceCell);
            }
        }
    
        // #region Test
        //
        // int coals = 0;
        // int irons = 0;
        // foreach (var resource in mine.Resources)
        // {
        //     if (resource.Variant == "Coal")
        //         coals++;
        //     else if (resource.Variant == "Iron")
        //         irons++;
        // }
        //     
        // Console.WriteLine($"no of root nodes: {numberOfRootNodes}");
        // Console.WriteLine($"Iron:{irons}, Coal:{coals}");
        //
        // #endregion

        await Task.Delay(500);
    }
    
    private Cell GetRandomCell(List<Cell> cells)
    {
        return cells[_rand.Next(0,cells.Count)];
    }
    
    private Cell GetRandomAdjacentCell(List<Cell> cells, Cell currentCell)
    {
        var xMin = currentCell.PositionX - 1;
        var xMax = currentCell.PositionX + 1;
        var yMin = currentCell.PositionY - 1;
        var yMax = currentCell.PositionY + 1;

        var adjacentCells = new List<Cell>();
        
        for (var i = xMin; i <= xMax; i++)
        {
            for (var j = yMin; j <= yMax; j++)
            {
                var cell = cells.FirstOrDefault(tempCell => tempCell.PositionX == i && tempCell.PositionY == j);
                if(cell == null) continue;
                if(cell == currentCell) continue;
                if (cell.HasResource || cell.HasCave || cell.HasArtifact
                    || !cell.IsInstantiated || cell.IsBroken || !cell.IsBreakable) continue;
                adjacentCells.Add(cell);
            }
        }

        return adjacentCells[_rand.Next(0, adjacentCells.Count - 1)];
    }

    
    private Resource AddResourceToMine(string variant, int posX, int posY, Mine mine)
    {
        var resources = _resourceDatabase.ToList();
        var resource = resources.FirstOrDefault(resource1 => resource1.Variant == variant);
        resource!.Id = Guid.NewGuid().ToString();
        resource.PositionX = posX;
        resource.PositionY = posY;
        mine.Resources.Add(resource);
        return resource;
    }
    
    private void FormResourceDistanceOfFourTiles(List<Cell> cells, Cell currentCell)
    {
        var xMin = currentCell.PositionX - 3;
        var xMax = currentCell.PositionX + 3;
        var yMin = currentCell.PositionY - 3;
        var yMax = currentCell.PositionY + 3;
        
        for (var i = xMin; i <= xMax; i++)
        {
            for (var j = yMin; j <= yMax; j++)
            {
                var cell = cells.FirstOrDefault(tempCell => tempCell.PositionX == i && tempCell.PositionY == j);
                if(cell == null) continue;
                if(cell != currentCell) continue;
                cells.Remove(cell);
            }
        }
    }

    #endregion
}