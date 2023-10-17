using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.ExhibitRepository;

public class ExhibitRepository : IExhibitRepository
{
    private readonly JsonFileDatabase<Exhibit> _exhibitDatabase;

    public ExhibitRepository(JsonFileDatabase<Exhibit> exhibitDatabase)
    {
        _exhibitDatabase = exhibitDatabase;
    }
 
    public async Task<Exhibit> Insert(Exhibit exhibit)
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        exhibits?.Add(exhibit);
        if (exhibits != null) await _exhibitDatabase.WriteDataAsync(exhibits);
        return exhibit;
    }

    public async Task<Exhibit> Update(string id, Exhibit exhibit)
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        var exhibitToUpdate = exhibit;
        exhibitToUpdate.Id = id;
        if (exhibits != null) await _exhibitDatabase.WriteDataAsync(exhibits);
        return exhibit;
    }

    public async Task<Exhibit?> GetById(string id)
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        var exhibit = exhibits!.FirstOrDefault(tile => tile.Id == id);
        return exhibit;
    }

    public async Task<List<Exhibit>?> GetAll()
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        return exhibits;
    }

    public async Task<Exhibit?> Delete(string id)
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        var exhibit = exhibits!.FirstOrDefault(tile => tile.Id == id);
        if (exhibit != null) exhibits?.Remove(exhibit);
        if (exhibits != null) await _exhibitDatabase.WriteDataAsync(exhibits);
        return exhibit;
    }
}