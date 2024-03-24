using System.Numerics;
using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ProceduralMineGenerationRepository;
using ProjectMuseum.Services.MineService.Sub_Services.CaveService;
using ProjectMuseum.Services.MineService.Sub_Services.MineArtifactService;
using ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService.MineOrdinaryCellGeneratorService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService;
using ProjectMuseum.Services.MineService.Sub_Services.ResourceService;
using ProjectMuseum.Services.MineService.Sub_Services.SiteArtifactChanceService;
using ProjectMuseum.Services.MineService.Sub_Services.SpecialBackdropService;

namespace ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService;

public class ProceduralMineGenerationService : IProceduralMineGenerationService
{
    private readonly IMineRepository _mineRepository;
    private readonly IMineService _mineService;
    private readonly IProceduralMineGenerationRepository _proceduralMineGenerationRepository;
    private readonly IMineOrdinaryCellGeneratorService _mineOrdinaryCellGeneratorService;
    private readonly ICaveGeneratorService _caveGeneratorService;
    private readonly IRawArtifactFunctionalService _rawArtifactFunctionalService;
    private readonly IMineArtifactService _mineArtifactService;
    private readonly ISiteArtifactChanceService _siteArtifactChanceService;
    private readonly IResourceService _resourceService;


    private readonly ISpecialBackdropService _specialBackdropService;

    private readonly JsonFileDatabase<SpecialBackdropPngInformation> _specialBackdropPngInformationDatabase;

    public ProceduralMineGenerationService(IProceduralMineGenerationRepository proceduralMineGenerationRepository,
        IMineOrdinaryCellGeneratorService mineOrdinaryCellGeneratorService, ICaveGeneratorService caveGeneratorService,
        IMineRepository mineRepository, ISpecialBackdropService specialBackdropService,
        JsonFileDatabase<SpecialBackdropPngInformation> specialBackdropPngInformationDatabase, IMineService mineService,
        IRawArtifactFunctionalService rawArtifactFunctionalService, IMineArtifactService mineArtifactService,
        ISiteArtifactChanceService siteArtifactChanceService, IResourceService resourceService)
    {
        _proceduralMineGenerationRepository = proceduralMineGenerationRepository;
        _mineOrdinaryCellGeneratorService = mineOrdinaryCellGeneratorService;
        _caveGeneratorService = caveGeneratorService;
        _mineRepository = mineRepository;
        _specialBackdropService = specialBackdropService;
        _specialBackdropPngInformationDatabase = specialBackdropPngInformationDatabase;
        _mineService = mineService;
        _rawArtifactFunctionalService = rawArtifactFunctionalService;
        _mineArtifactService = mineArtifactService;
        _siteArtifactChanceService = siteArtifactChanceService;
        _resourceService = resourceService;
    }

    public async Task<Mine> GenerateProceduralMine()
    {
        await GenerateMineOrdinaryCells();
        await GenerateBossCave();
        await GenerateCaves();
        await GenerateSpecialBackdrops();
        await GenerateArtifacts();
        await GenerateResources();
        // await GenerateUnbreakableRocks();
        var mine = await _mineRepository.Get();
        return mine;
    }

    public async Task GenerateMineOrdinaryCells()
    {
        var mineGenData = await _proceduralMineGenerationRepository.GetProceduralMineGenerationData();
        var mine = await _mineOrdinaryCellGeneratorService.GenerateMineCellData(mineGenData.MineSizeX,
            mineGenData.MineSizeY, mineGenData.CellSize);
    }

    #region Generate Caves

    public async Task GenerateBossCave()
    {
        var mineGenData = await _proceduralMineGenerationRepository.GetProceduralMineGenerationData();
        var mine = await _mineService.GetMineData();

        foreach (var cell in mine.Cells)
            cell.HasCave = false;
        mine.Caves = new List<Cave>();

        var bossCaveSizeX = mineGenData.BossCaveSizeX;
        var bossCaveSizeY = mineGenData.BossCaveSizeY;

        var yAxisBottomIndex = mineGenData.MineSizeY - 2;
        var xAxisCenterIndex = mineGenData.MineSizeX / 2;

        var xMin = xAxisCenterIndex - bossCaveSizeX / 2;
        var xMax = xAxisCenterIndex + bossCaveSizeX / 2;
        var yMin = yAxisBottomIndex - bossCaveSizeY;
        var yMax = yAxisBottomIndex;

        var noOfStalactites = Math.Clamp(mineGenData.StalactiteCount, 0, bossCaveSizeX);
        var noOfStalagmites = Math.Clamp(mineGenData.StalagmiteCount, 0, bossCaveSizeX);

        var cave = await _caveGeneratorService.GenerateCave(xMin, xMax, yMin, yMax, noOfStalagmites, noOfStalactites);
    }

    public async Task GenerateCaves()
    {
        var rand = new Random();
        var mine = await _mineRepository.Get();
        var mineGenData = await _proceduralMineGenerationRepository.GetProceduralMineGenerationData();

        var mineX = mine.GridWidth;
        var mineY = mine.GridLength;

        #region cavesToGenerate contains list of cave dimensions tha has to be generated

        var noOfCaves = rand.Next(mineGenData.NumberOfMaxCaves / 2, mineGenData.NumberOfMaxCaves);
        var cavesToGenerate = new List<Vector2>();
        for (var i = 0; i < noOfCaves; i++)
        {
            var tempX = rand.Next(mineGenData.CaveMinSizeX, mineGenData.CaveMaxSizeX);
            var tempY = rand.Next(mineGenData.CaveMinSizeY, mineGenData.CaveMaxSizeY);
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

            Console.WriteLine($"cave dim: ({tempCave.X},{tempCave.Y})");
            var cave = await _caveGeneratorService.GenerateCave(xMin, xMax, yMin, yMax, stalagmites, stalactites);
            Console.WriteLine(
                $"Cave generated xMin:{cave.LeftBound}, xMax:{cave.RightBound}. yMin:{cave.TopBound}, yMax:{cave.BottomBound}");
        }

        #endregion
    }

    #endregion

    #region Generate Special Backdrops

    public async Task GenerateSpecialBackdrops()
    {
        var backdropSlotsX = 3;
        var backdropSlotsY = 3;
        var noOfBackdrops = 2;

        var mine = await _mineRepository.Get();
        var mineSizeX = mine.GridLength;
        var mineSizeY = mine.GridWidth;
        var offsetX = mineSizeX / backdropSlotsX / 2;
        var offsetY = mineSizeY / backdropSlotsY / 2;

        var listOfCoords = new List<Vector2>();

        for (var i = 0; i < backdropSlotsX; i++)
        {
            for (int j = 0; j < backdropSlotsY; j++)
            {
                var xPos = i * mineSizeX + offsetX;
                var yPos = j * mineSizeY + offsetY;
                var coord = new Vector2(xPos, yPos);
                if (listOfCoords.Contains(coord)) continue;
                listOfCoords.Add(coord);
            }
        }

        var listOfBackdrops = await _specialBackdropPngInformationDatabase.ReadDataAsync();
        if (listOfBackdrops == null) return;

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

        await _specialBackdropService.SetSpecialBackdrops(listOfAddedBackdrops);
    }

    #endregion

    #region Generate Artifacts

    public async Task GenerateArtifacts()
    {
        var mineGenData = await _proceduralMineGenerationRepository.GetProceduralMineGenerationData();
        var rawArtifactFunctionals = await _rawArtifactFunctionalService.GetAllRawArtifactFunctional();
        
        var siteArtifactChance = await _siteArtifactChanceService.GetSiteArtifactChanceDataBySite(mineGenData.Site);
        var rand = new Random();
        var regionalArtifacts = rawArtifactFunctionals!
            .Where(rawArtifactFunctional => rawArtifactFunctional.Region == mineGenData.Region).ToList();
        
        var weaponCount = (int)(siteArtifactChance.Weapon * mineGenData.TotalNoOfArtifacts);
        var armorCount = (int) (siteArtifactChance.Armor * mineGenData.TotalNoOfArtifacts);
        var clothingCount = (int) (siteArtifactChance.Clothing * mineGenData.TotalNoOfArtifacts);
        var economicCount = (int) (siteArtifactChance.Economic * mineGenData.TotalNoOfArtifacts);
        var vesselCount = (int) (siteArtifactChance.Vessel * mineGenData.TotalNoOfArtifacts);
        var leisureCount = (int) (siteArtifactChance.Leisure * mineGenData.TotalNoOfArtifacts);
        var toolCount = (int) (siteArtifactChance.Tool * mineGenData.TotalNoOfArtifacts);
        var ceremonialCount = (int) (siteArtifactChance.Ceremonial * mineGenData.TotalNoOfArtifacts);
        var legendaryCount = (int) (siteArtifactChance.Legendary * mineGenData.TotalNoOfArtifacts);
        
        var listOfRawArtifacts = new List<RawArtifactFunctional>();

        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Weapon").ToList()
            .OrderBy(x => rand.Next()).Take(weaponCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Armor").ToList()
            .OrderBy(x => rand.Next()).Take(armorCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Clothing").ToList()
            .OrderBy(x => rand.Next()).Take(clothingCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Economic").ToList()
            .OrderBy(x => rand.Next()).Take(economicCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Vessel").ToList()
            .OrderBy(x => rand.Next()).Take(vesselCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Leisure").ToList()
            .OrderBy(x => rand.Next()).Take(leisureCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Tool").ToList()
            .OrderBy(x => rand.Next()).Take(toolCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Ceremonial").ToList()
            .OrderBy(x => rand.Next()).Take(ceremonialCount));
        listOfRawArtifacts.AddRange(regionalArtifacts.Where(rawArtifact => rawArtifact.ObjectClass == "Legendary").ToList()
            .OrderBy(x => rand.Next()).Take(legendaryCount));
        
        Console.WriteLine($"raw artifact functional: {rawArtifactFunctionals!.Count}");
        Console.WriteLine($"list of raw artifacts: {listOfRawArtifacts.Count}");

        var listOfArtifacts = new List<Artifact>();
        foreach (var artifactFunctional in listOfRawArtifacts)
        {
            var artifact = new Artifact
            {
                Id = Guid.NewGuid().ToString(),
                RawArtifactId = artifactFunctional.Id
            };
            
            listOfArtifacts.Add(artifact);
        }
        
        var mine = await _mineService.GetMineData();
        var cells = mine.Cells;
        var cellsToRemove = new List<Cell>();
        foreach (var cell in cells)
        {
            if (cell.IsBroken || cell.HasCave || !cell.IsInstantiated || !cell.IsBreakable) 
                cellsToRemove.Add(cell);
        }

        foreach (var cell in cellsToRemove)
        {
            cells.Remove(cell);
        }

        foreach (var artifact in listOfArtifacts)
        {
            var cell = cells[rand.Next(0, cells.Count)];
            artifact.PositionX = cell.PositionX;
            artifact.PositionY = cell.PositionY;
            cells.Remove(cell);
        }
        
        var artifacts = await _mineArtifactService.GenerateNewArtifacts(listOfArtifacts);
        foreach (var artifact in artifacts)
            Console.WriteLine($"id: {artifact.RawArtifactId}, X: {artifact.PositionX}, Y: {artifact.PositionY}");
        
        await _mineService.AssignArtifactsToMine();
    }

    #endregion

    public async Task<List<Resource>> GenerateResources()
    {
        var mineGenData = await _proceduralMineGenerationRepository.GetProceduralMineGenerationData();
        var resources = await _resourceService.GenerateResources(mineGenData.ResourceVariants);
        return resources;
    }

    public Task GenerateUnbreakableRocks()
    {
        throw new NotImplementedException();
    }
}