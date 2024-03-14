using System.Numerics;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ProceduralMineGenerationRepository;
using ProjectMuseum.Services.MineService.Sub_Services.CaveService;
using ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService.MineOrdinaryCellGeneratorService;

namespace ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService;

public class ProceduralMineGenerationService : IProceduralMineGenerationService
{
    private readonly IMineRepository _mineRepository;
    private readonly IProceduralMineGenerationRepository _proceduralMineGenerationRepository;
    private readonly IMineOrdinaryCellGeneratorService _mineOrdinaryCellGeneratorService;
    private readonly ICaveGeneratorService _caveGeneratorService;
    
    public ProceduralMineGenerationService(IProceduralMineGenerationRepository proceduralMineGenerationRepository, IMineOrdinaryCellGeneratorService mineOrdinaryCellGeneratorService, ICaveGeneratorService caveGeneratorService, IMineRepository mineRepository)
    {
        _proceduralMineGenerationRepository = proceduralMineGenerationRepository;
        _mineOrdinaryCellGeneratorService = mineOrdinaryCellGeneratorService;
        _caveGeneratorService = caveGeneratorService;
        _mineRepository = mineRepository;
    }

    public async Task<Mine> GenerateProceduralMine()
    {
        await GenerateMineOrdinaryCells();
        await GenerateBossCave();
        await GenerateCaves();
        await GenerateSpecialBackdrops();
        await GenerateArtifacts();
        await GenerateResources();
        await GenerateUnbreakableRocks();
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
        var caves = mine.Caves;
        var mineX = mine.GridWidth;
        var mineY = mine.GridLength;
        var occupiedCaveCells = new List<Vector2>();
        var possibleCaveCells = new List<Vector2>();

        #region Populating possible cave cells with all mine cells

        for (var i = 0; i < mine.GridWidth -1; i++)
        {
            for (var j = 0; j < mine.GridLength; j++)
            {
                var cellPos = new Vector2(i, j);
                if(possibleCaveCells.Contains(cellPos)) continue;
                possibleCaveCells.Add(cellPos);
            }
        }

        #endregion   
        
        var minCaveDistance = mineGenData.MinDistanceBetweenCaves/2;

        #region Filter possible cave cells by removing occupied cells (including cave distance and border)

        foreach (var cave in caves)
        {
            var leftBound = cave.LeftBound - minCaveDistance;
            var rightBound = cave.RightBound + minCaveDistance;
            var topBound = cave.TopBound - minCaveDistance;
            var bottomBound = cave.BottomBound + minCaveDistance;

            for (var i = leftBound; i <= rightBound; i++)
            {
                for (var j = topBound; j < bottomBound; j++)
                {
                    var tempCellPos = new Vector2(i, j);
                    possibleCaveCells.Remove(tempCellPos);
                    if(!occupiedCaveCells.Contains(tempCellPos))
                        occupiedCaveCells.Add(tempCellPos);
                }
            }

            foreach (var caveCell in possibleCaveCells)
            {
                if (caveCell.X != 0 && (int)caveCell.X != mineGenData.MineSizeX - 1 &&
                    caveCell.Y != 0 && (int)caveCell.Y != mineGenData.MineSizeY - 1) continue;
                possibleCaveCells.Remove(caveCell);
                if (occupiedCaveCells.Contains(caveCell)) continue;
                occupiedCaveCells.Add(caveCell);
            }
        }

        #endregion

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

        #region Sorting Caves from highest to lowest

        var sortedCavesToGenerate = cavesToGenerate.OrderBy(cave => cave.X * cave.Y).ToList();
        sortedCavesToGenerate.Reverse();

        #endregion

        #region Utilities

        List<Vector4> DeterminePossibleContinuedCells()
        {
            var continuedCells = new List<Vector4>();
            foreach (var caveCell in possibleCaveCells)
            {
                var tempCell = continuedCells.FirstOrDefault(temp =>
                    caveCell.X >= temp.X && caveCell.X <= temp.Z && caveCell.Y >= temp.Y && caveCell.Y <= temp.W);
                if(continuedCells.Contains(tempCell)) continue;
                var initialPos = caveCell;
                var initialX = (int)initialPos.X;
                var initialY = (int)initialPos.Y;
                var finalX = (int)occupiedCaveCells!.FirstOrDefault(occupied => (int)occupied.Y == initialY).X - 1;
                var finalY = (int)occupiedCaveCells!.FirstOrDefault(occupied => (int)occupied.X == finalX).Y - 1;
                continuedCells.Add(new Vector4(initialX,initialY, finalX, finalY));
            }

            return continuedCells;
        }

        #endregion
    }

    public Task GenerateSpecialBackdrops()
    {
        throw new NotImplementedException();
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