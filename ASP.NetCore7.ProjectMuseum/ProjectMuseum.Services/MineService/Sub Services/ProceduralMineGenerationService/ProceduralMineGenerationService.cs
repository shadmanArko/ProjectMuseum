using System.Numerics;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ProceduralMineGenerationRepository;
using ProjectMuseum.Services.MineService.Sub_Services.CaveService;
using ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService.MineOrdinaryCellGeneratorService;
using ProjectMuseum.Services.MineService.Sub_Services.SpecialBackdropService;

namespace ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService;

public class ProceduralMineGenerationService : IProceduralMineGenerationService
{
    private readonly IMineRepository _mineRepository;
    private readonly IProceduralMineGenerationRepository _proceduralMineGenerationRepository;
    private readonly IMineOrdinaryCellGeneratorService _mineOrdinaryCellGeneratorService;
    private readonly ICaveGeneratorService _caveGeneratorService;

    private readonly ISpecialBackdropService _specialBackdropService;

    private readonly JsonFileDatabase<SpecialBackdropPngInformation> _specialBackdropPngInformationDatabase;
    
    public ProceduralMineGenerationService(IProceduralMineGenerationRepository proceduralMineGenerationRepository, IMineOrdinaryCellGeneratorService mineOrdinaryCellGeneratorService, ICaveGeneratorService caveGeneratorService, IMineRepository mineRepository, ISpecialBackdropService specialBackdropService, JsonFileDatabase<SpecialBackdropPngInformation> specialBackdropPngInformationDatabase)
    {
        _proceduralMineGenerationRepository = proceduralMineGenerationRepository;
        _mineOrdinaryCellGeneratorService = mineOrdinaryCellGeneratorService;
        _caveGeneratorService = caveGeneratorService;
        _mineRepository = mineRepository;
        _specialBackdropService = specialBackdropService;
        _specialBackdropPngInformationDatabase = specialBackdropPngInformationDatabase;
    }

    public async Task<Mine> GenerateProceduralMine()
    {
        await GenerateMineOrdinaryCells();
        await GenerateBossCave();
        await GenerateCaves();
        await GenerateSpecialBackdrops();
        // await GenerateArtifacts();
        // await GenerateResources();
        // await GenerateUnbreakableRocks();
        var mine = await _mineRepository.Get();
        return mine;
    }
    
    public async Task GenerateMineOrdinaryCells()
    {
        var mineGenData = await _proceduralMineGenerationRepository.GetProceduralMineGenerationData();
        var mine = await _mineOrdinaryCellGeneratorService.GenerateMineCellData(mineGenData.MineSizeX, mineGenData.MineSizeY, mineGenData.CellSize);
    }

    public async Task GenerateBossCave()
    {
        var mineGenData = await _proceduralMineGenerationRepository.GetProceduralMineGenerationData();

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

        var cave = await _caveGeneratorService.GenerateCave(xMin, xMax, yMin, yMax, noOfStalagmites,noOfStalactites);
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
                var xPos = Math.Clamp(caveSlotPosX * i + offsetX, 1, mineX -1);
                var yPos = Math.Clamp(caveSlotPosY * j + offsetY, 1, mineY -2);
                
                var arbitraryX = xPos + rand.Next(-xPos / 2, xPos / 2);
                var arbitraryY = yPos + rand.Next(-yPos / 2, yPos / 2);
            
                var coord = new Vector2(arbitraryX, arbitraryY);
                if (listOfCoords.Contains(coord))
                {
                    i--;
                    continue;
                }
                listOfCoords.Add(coord);
                // Console.WriteLine($"coord added: x={xPos}, y={yPos}");
            }
        }

        #endregion

        #region Create Cave
        
        foreach (var tempCave in cavesToGenerate)
        {
            var coord = listOfCoords[rand.Next(0, listOfCoords.Count)];
            var xMin = Math.Clamp((int) coord.X, 1, mineX -1);
            var yMin = Math.Clamp((int) coord.Y, 1, mineY - 2);
            var xMax = Math.Clamp((int)(coord.X + tempCave.X), 1, mineX - 1);
            var yMax = Math.Clamp((int) (coord.Y + tempCave.Y), 1, mineY - 2);
            listOfCoords.Remove(coord);
            
            var stalagmites = rand.Next(2, xMax - xMin);
            var stalactites = rand.Next(2, xMax - xMin);
        
            Console.WriteLine($"cave dim: ({tempCave.X},{tempCave.Y})");
            var cave = await _caveGeneratorService.GenerateCave(xMin, xMax, yMin, yMax, stalagmites, stalactites);
            Console.WriteLine($"Cave generated xMin:{cave.LeftBound}, xMax:{cave.RightBound}. yMin:{cave.TopBound}, yMax:{cave.BottomBound}");
            
        }
        
        #endregion
    }

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
                if(listOfCoords.Contains(coord)) continue;
                listOfCoords.Add(coord);
            }
        }

        var listOfBackdrops = await _specialBackdropPngInformationDatabase.ReadDataAsync();
        if(listOfBackdrops == null) return;

        var rand = new Random();
        var listOfAddedBackdrops = new List<SpecialBackdropPngInformation>();

        for (var i = 0; i < noOfBackdrops; i++)
        {
            var tempBackdrop = listOfBackdrops[rand.Next(0, listOfBackdrops.Count)];
            var tempCoord = listOfCoords[rand.Next(0, listOfCoords.Count)];
            
            tempBackdrop.TilePositionX = (int) tempCoord.X;
            tempBackdrop.TilePositionY = (int) tempCoord.Y;
            
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

    public Task GenerateArtifacts()
    {
        throw new NotImplementedException();
    }

    public Task GenerateResources()
    {
        throw new NotImplementedException();
    }

    public Task GenerateUnbreakableRocks()
    {
        throw new NotImplementedException();
    }
}