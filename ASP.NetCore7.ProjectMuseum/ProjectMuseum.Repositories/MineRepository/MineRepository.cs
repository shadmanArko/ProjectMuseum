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

    public async Task<Mine> Update(Mine mine)
    {
        var mines = await _mineDatabase.ReadDataAsync();
        if (mines != null)
        {
            mines[0] = mine;
            await _mineDatabase.WriteDataAsync(mines);
        }

        return mine;
    }
}