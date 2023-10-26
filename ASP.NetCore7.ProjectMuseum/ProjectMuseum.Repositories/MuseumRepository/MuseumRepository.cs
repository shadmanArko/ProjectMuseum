using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository;

public class MuseumRepository : IMuseumRepository
{
    private readonly JsonFileDatabase<Museum> _museumDatabase;

    public MuseumRepository(JsonFileDatabase<Museum> museumDatabase)
    {
        _museumDatabase = museumDatabase;
    }
    public Task<Museum> Insert(Museum museum)
    {
        throw new NotImplementedException();
    }

    public Task<Museum> Update(string id, Museum museum)
    {
        throw new NotImplementedException();
    }

    public async Task<Museum?> GetById(string id)
    {
        var museums = await _museumDatabase.ReadDataAsync();
        var museum = museums!.FirstOrDefault(tile => tile.Id == id);
        return museum;
    }

    public Task<List<Museum>?> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetMuseumBalance(string id)
    {
        var museums = await _museumDatabase.ReadDataAsync();
        var museum = museums!.FirstOrDefault(tile => tile.Id == id);
        return museum!.Money;
    }

    public Task<Museum?> Delete(string id)
    {
        throw new NotImplementedException();
    }
}