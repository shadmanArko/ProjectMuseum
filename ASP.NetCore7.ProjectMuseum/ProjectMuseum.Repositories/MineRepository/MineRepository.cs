using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository;

public class MineRepository : IMineRepository
{
    private readonly JsonFileDatabase<Mine> _mineDatabase;

    public MineRepository(JsonFileDatabase<Mine> mineDatabase)
    {
        _mineDatabase = mineDatabase;
    }

    public async Task<Mine> Get()
    {
        var mine = await _mineDatabase.ReadDataAsync();
        return mine[0];
    }
}