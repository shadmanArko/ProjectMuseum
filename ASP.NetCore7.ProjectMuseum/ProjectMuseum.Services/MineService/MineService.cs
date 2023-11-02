using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;

namespace ProjectMuseum.Services.MineService;

public class MineService : IMineService
{
    private readonly MineRepository _mineRepository;

    public MineService(MineRepository mineRepository)
    {
        _mineRepository = mineRepository;
    }

    public async Task<Mine> GetMineData()
    {
        return await _mineRepository.Get();
    }
}