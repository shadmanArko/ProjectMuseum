using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.MineCellService;

public class MineCellGenerator
{
    public int XSize = 20;
    public int YSize = 20;
    private readonly IMineRepository _mineRepository;


    public MineCellGenerator(IMineRepository mineRepository)
    {
        _mineRepository = mineRepository;
    }

    public void GenerateMineCellData()
    {
        var mine = new Mine();
        
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
            }
        }

        _mineRepository.Update(mine);

    }
}