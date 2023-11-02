using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;

namespace ProjectMuseum.Services.MineService;

public class MineService : IMineService
{
    private readonly IMineRepository _mineRepository;

    public MineService(IMineRepository mineRepository)
    {
        _mineRepository = mineRepository;
    }

    public async Task<Mine> UpdateMine(Mine mine)
    {
        await _mineRepository.Update(mine);
        return mine;
    }

    public async Task<Mine> GetMineData()
    {
        var mine = await _mineRepository.Get();
        return mine;
    }
}