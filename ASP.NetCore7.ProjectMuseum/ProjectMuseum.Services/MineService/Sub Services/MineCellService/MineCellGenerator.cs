using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.MineCellService;

public class MineCellGenerator : IMineCellGenerator
{
    public int XSize = 49;
    public int YSize = 64;
    private readonly IMineRepository _mineRepository;


    public MineCellGenerator(IMineRepository mineRepository)
    {
        _mineRepository = mineRepository;
    }

    public async Task<Mine> GenerateMineCellData()
    {
        var mine = new Mine();
        var cells = new List<Cell>();
        
        for (int i = 0; i < XSize; i++)
        {
            for (int j = 0; j < YSize; j++)
            {
                var cell = new Cell
                {
                    Id = Guid.NewGuid().ToString(),
                    PositionX = i,
                    PositionY = j
                };
                cells.Add(cell);
            }
        }

        mine.Cells = cells;

        return await _mineRepository.Update(mine);

    }
}