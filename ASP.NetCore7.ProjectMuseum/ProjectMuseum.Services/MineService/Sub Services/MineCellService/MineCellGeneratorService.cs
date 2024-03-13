using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CaveRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.MineCellService;

public class MineCellGeneratorService : IMineCellGeneratorService
{
    public int XSize = 49;
    public int YSize = 64;
    public int cellSize = 20;
    private readonly IMineRepository _mineRepository;
    private readonly ICaveRepository _caveRepository;


    public MineCellGeneratorService(IMineRepository mineRepository, ICaveRepository caveRepository)
    {
        _mineRepository = mineRepository;
        _caveRepository = caveRepository;
    }

    public async Task<Mine> GenerateMineCellData()
    {
        var mine = new Mine
        {
            CellSize = cellSize,
            GridWidth = XSize,
            GridLength = YSize,
            Caves = new List<Cave>(),
            WallPlaceables = new List<WallPlaceable>(),
            CellPlaceables = new List<CellPlaceable>(),
            SpecialBackdropPngInformations = new List<SpecialBackdropPngInformation>(),
            Resources = new List<Resource>()
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
        mine.Caves = new List<Cave>();
        
        return await _mineRepository.Update(mine);
    }

    private void CreateBlankCell(Cell cell)
    {
        cell.IsBreakable = false;
        cell.IsBroken = false;
        cell.IsInstantiated = false;
        cell.HasArtifact = false;
        cell.HasCave = false;
        cell.HitPoint = 10000;
    }

    private void CreateUnbreakableCell(Cell cell)
    {
        cell.IsBreakable = false;
        cell.IsBroken = false;
        cell.IsInstantiated = true;
        cell.HasArtifact = false;
        cell.HasCave = false;
        cell.HitPoint = 10000;
    }

    private void CreateBreakableCell(Cell cell)
    {
        cell.IsBreakable = true;
        cell.IsBroken = false;
        cell.IsInstantiated = true;
        cell.IsRevealed = false;
        cell.HasArtifact = false;
        cell.HasCave = false;
        cell.HitPoint = 4;
    }
}