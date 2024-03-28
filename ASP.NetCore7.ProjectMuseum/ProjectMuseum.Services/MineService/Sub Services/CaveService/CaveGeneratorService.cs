using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CaveRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.CaveService;

public class CaveGeneratorService : ICaveGeneratorService
{
    private readonly IMineService _mineService;
    private readonly ICaveGeneratorRepository _caveGeneratorRepository;

    public CaveGeneratorService(ICaveGeneratorRepository caveGeneratorRepository, IMineService mineService)
    {
        _caveGeneratorRepository = caveGeneratorRepository;
        _mineService = mineService;
    }
    
    public async Task<Cave> GenerateCave(int xMin, int xMax, int yMin, int yMax, int stalagmiteCount, int stalactiteCount)
    {
        var mine = await _mineService.GetMineData();
        
        var cells = mine.Cells;
        var caveCellIds = new List<string>();
        var possibleStalagmiteCells = new List<Cell>();
        var possibleStalactiteCells = new List<Cell>();

        for (var i = xMin; i <= xMax; i++)
        {
            for (var j = yMin; j <= yMax; j++)
            {
                var cell = cells.FirstOrDefault(tempCell => tempCell.PositionX == i && tempCell.PositionY == j);
                if(cell == null) continue;
                
                cell.HasCave = true;
                cell.IsBroken = true;
                cell.TopBrokenSide = true;
                cell.BottomBrokenSide = true;
                cell.LeftBrokenSide = true;
                cell.RightBrokenSide = true;
                
                if(caveCellIds.Contains(cell.Id)) continue;
                caveCellIds.Add(cell.Id);
                if (cell.PositionY == yMin)
                    possibleStalactiteCells.Add(cell);
                else if(cell.PositionY == yMax)
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
            if(newCave.StalagmiteCellIds.Contains(cell.Id)) continue;
            newCave.StalagmiteCellIds.Add(cell.Id);
        }
        Console.WriteLine($"stalagmite cell count: {possibleStalagmiteCells.Count}");
        
        for (var numberOfStalactites = stalactiteCount; numberOfStalactites > 0; numberOfStalactites--)
        {
            var cell = possibleStalactiteCells[rand.Next(0, possibleStalactiteCells.Count)];
            if(newCave.StalactiteCellIds.Contains(cell.Id)) continue;
            newCave.StalactiteCellIds.Add(cell.Id);
        }
        Console.WriteLine($"stalactite cell count: {possibleStalactiteCells.Count}");
        
        mine.Caves.Add(newCave);
        await _mineService.UpdateMine(mine);
        return newCave;
    }
}