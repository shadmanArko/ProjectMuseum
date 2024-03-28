using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CaveRepository;

public class CaveGeneratorRepository : ICaveGeneratorRepository
{
    private readonly IMineRepository _mineRepository;
    private readonly JsonFileDatabase<Cave> _caveDatabase;

    public CaveGeneratorRepository(IMineRepository mineRepository, JsonFileDatabase<Cave> caveDatabase)
    {
        _mineRepository = mineRepository;
        _caveDatabase = caveDatabase;
    }

    public async Task<Cave> AddCave(Cave cave)
    {
        var mine = await _mineRepository.Get();
        mine.Caves.Add(cave);
        await _mineRepository.Update(mine);
        return cave;
    }
}