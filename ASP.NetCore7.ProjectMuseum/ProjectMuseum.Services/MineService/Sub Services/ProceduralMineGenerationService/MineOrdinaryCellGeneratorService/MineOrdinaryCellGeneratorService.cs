using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CaveRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService.MineOrdinaryCellGeneratorService;

public class MineOrdinaryCellGeneratorService : IMineOrdinaryCellGeneratorService
{
    private int _maxHitPoint = 40;
    private readonly IMineRepository _mineRepository;
    private readonly ICaveGeneratorRepository _caveGeneratorRepository;


    public MineOrdinaryCellGeneratorService(IMineRepository mineRepository, ICaveGeneratorRepository caveGeneratorRepository)
    {
        _mineRepository = mineRepository;
        _caveGeneratorRepository = caveGeneratorRepository;
    }

    public async Task<Mine> GenerateMineCellData(int xSize, int ySize, int cellSize)
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

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                var cell = new Cell
                {
                    Id = Guid.NewGuid().ToString(),
                    PositionX = x,
                    PositionY = y
                };
                if (y == 0 || y == ySize-1)
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
                else if (x == 0 || x == xSize -1)
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
        
        return await _mineRepository.Update(mine);
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
}