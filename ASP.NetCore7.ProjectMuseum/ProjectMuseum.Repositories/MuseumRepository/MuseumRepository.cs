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

    public async Task<float> GetMuseumBalance(string id)
    {
        var museums = await _museumDatabase.ReadDataAsync();
        var museum = museums!.FirstOrDefault(tile => tile.Id == id);
        return museum!.Money;
    }

    public async Task<Museum> ReduceMuseumBalance(string id, float amount)
    {
        var museums = await _museumDatabase.ReadDataAsync();
        var museum = museums!.FirstOrDefault(tile => tile.Id == id);
        museum!.Money -= amount;
        if (museums != null) await _museumDatabase.WriteDataAsync(museums);
        return museum;
    }

    public async Task<Museum> AddToMuseumBalance(string id, float amount)
    {
        var museums = await _museumDatabase.ReadDataAsync();
        var museum = museums!.FirstOrDefault(tile => tile.Id == id);
        museum!.Money += amount;
        if (museums != null) await _museumDatabase.WriteDataAsync(museums);
        return museum;
    }

    public Task<Museum?> Delete(string id)
    {
        throw new NotImplementedException();
    }
}