using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.MineCellService;

public class MineCellGeneratorService : IMineCellGeneratorService
{
    public int XSize = 49;
    public int YSize = 64;
    public int cellSize = 20;
    private readonly IMineRepository _mineRepository;


    public MineCellGeneratorService(IMineRepository mineRepository)
    {
        _mineRepository = mineRepository;
    }

    public async Task<Mine> GenerateMineCellData()
    {
        var mine = new Mine
        {
            CellSize = cellSize,
            GridWidth = XSize,
            GridLength = YSize
        };
        var cells = new List<Cell>();

        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                var cell = new Cell
                {
                    Id = Guid.NewGuid().ToString(),
                    PositionX = x,
                    PositionY = y
                };
                if (y == 0 || y == YSize-1)
                {
                    if (y == 0 && x == XSize / 2)
                    {
                        CreateBlankCell(cell);
                        cells.Add(cell);
                        continue;
                    }
                    
                    CreateUnbreakableCell(cell);
                    cells.Add(cell);
                }
                else if (x == 0 || x == XSize -1)
                {
                    CreateUnbreakableCell(cell);
                    cells.Add(cell);
                }
                else
                {
                    CreateBreakableCell(cell);
                    cells.Add(cell);
                    if (y == 1 && x == XSize / 2)
                        cell.IsRevealed = true;
                }
            }
        }

        mine.Cells = cells;
        CreateCave(mine);
        return await _mineRepository.Update(mine);
    }

    private void CreateBlankCell(Cell cell)
    {
        cell.IsBreakable = false;
        cell.IsInstantiated = false;
        cell.HasArtifact = false;
        cell.HitPoint = 10000;
    }

    private void CreateUnbreakableCell(Cell cell)
    {
        cell.IsBreakable = false;
        cell.IsInstantiated = true;
        cell.HasArtifact = false;
        cell.HitPoint = 10000;
    }

    private void CreateBreakableCell(Cell cell)
    {
        cell.IsBreakable = true;
        cell.IsInstantiated = true;
        cell.IsRevealed = false;
        cell.HasArtifact = false;
        cell.HitPoint = 4;
    }

    private void CreateCave(Mine mine)
    {
        var caveCellIds = new List<string>();
        for (int i = 20; i < 30; i++)
        {
            for (int j = 10; j < 15; j++)
            {
                var cell = mine.Cells.FirstOrDefault(tempCell => tempCell.PositionX == i && tempCell.PositionY == j);
                if(cell == null) continue;
                cell.HasCave = true;
                cell.IsBroken = true;
                caveCellIds.Add(cell.Id!);
            }
        }

        var cave = new Cave
        {
            Id = Guid.NewGuid().ToString(),
            CellIds = caveCellIds
        };

        mine.Caves.Add(cave);
    }
}