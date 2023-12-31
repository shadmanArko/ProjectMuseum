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
                else if (x == XSize / 2 && y == 5)
                {
                    CreateArtifactCell(cell);
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

        return await _mineRepository.Update(mine);

    }

    private void CreateBlankCell(Cell cell)
    {
        cell.IsBreakable = false;
        cell.IsInstantiated = false;
        cell.HitPoint = 10000;
    }

    private void CreateUnbreakableCell(Cell cell)
    {
        cell.IsBreakable = false;
        cell.IsInstantiated = true;
        cell.HitPoint = 10000;
    }

    private void CreateBreakableCell(Cell cell)
    {
        cell.IsBreakable = true;
        cell.IsInstantiated = true;
        cell.IsRevealed = false;
        cell.HitPoint = 4;
    }

    private void CreateArtifactCell(Cell cell)
    {
        cell.IsBreakable = true;
        cell.IsInstantiated = true;
        cell.IsRevealed = false;
        cell.HasArtifact = true;
        cell.HitPoint = 3;
    }
}