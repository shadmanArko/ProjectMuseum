using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CaveRepository;

public class CaveRepository : ICaveRepository
{
    private readonly IMineRepository _mineRepository;
    private readonly JsonFileDatabase<Cave> _caveDatabase;

    public CaveRepository(IMineRepository mineRepository, JsonFileDatabase<Cave> caveDatabase)
    {
        _mineRepository = mineRepository;
        _caveDatabase = caveDatabase;
    }

    public async Task<Cave> GenerateCave(int xMin, int xMax, int yMin, int yMax)
    {
        var mine = await _mineRepository.Get();
        foreach (var cell in mine.Cells)
            cell.HasCave = false;
        mine.Caves.Clear();
        
        var cells = mine.Cells;
        var caveCellIds = new List<string>();
        var possibleStalagmiteCells = new List<Cell>();
        var possibleStalactiteCells = new List<Cell>();

        for (var i = xMin; i <= xMax; i++)
        {
            for (var j = yMin; j <= yMax; j++)
            {
                var cell = cells.FirstOrDefault(tempCell => tempCell.PositionX == i && tempCell.PositionY == j);
                Console.WriteLine($"cells is null: {cell == null}");
                if(cell == null) continue;
                
                cell.HasCave = true;
                cell.IsBroken = true;
                cell.TopBrokenSide = true;
                cell.BottomBrokenSide = true;
                cell.LeftBrokenSide = true;
                cell.RightBrokenSide = true;
                
                if(caveCellIds.Contains(cell.Id)) continue;
                caveCellIds.Add(cell.Id);
                Console.WriteLine($"cell Y:{cell.PositionY}, yMin:{yMin}");
                Console.WriteLine($"cell Y:{cell.PositionY}, yMax:{yMax}");
                if (cell.PositionY == yMin)
                    possibleStalactiteCells.Add(cell);
                else if(cell.PositionY == yMax)
                    possibleStalagmiteCells.Add(cell);
            }
        }
        
        var newCave = new Cave
        {
            Id = Guid.NewGuid().ToString(),
            CellIds = caveCellIds,
            StalagmiteCellIds = new List<string>(),
            StalactiteCellIds = new List<string>()
        };
        var rand = new Random();
        for (var numberOfStalagmites = rand.Next(0, possibleStalagmiteCells.Count); numberOfStalagmites > 0; numberOfStalagmites--)
        {
            Console.WriteLine($"stalagmite cell count: {possibleStalagmiteCells.Count}");
            var cell = possibleStalagmiteCells[rand.Next(0, possibleStalagmiteCells.Count)];
            if(newCave.StalagmiteCellIds.Contains(cell.Id)) continue;
            newCave.StalagmiteCellIds.Add(cell.Id);
        }
        
        for (var numberOfStalactites = rand.Next(0,possibleStalactiteCells.Count); numberOfStalactites > 0; numberOfStalactites--)
        {
            Console.WriteLine($"stalactite cell count: {possibleStalactiteCells.Count}");
            var cell = possibleStalactiteCells[rand.Next(0, possibleStalactiteCells.Count)];
            if(newCave.StalactiteCellIds.Contains(cell.Id)) continue;
            newCave.StalactiteCellIds.Add(cell.Id);
        }
        
        mine.Caves.Add(newCave);
        await _mineRepository.Update(mine);
        return newCave;
    }
}