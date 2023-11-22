using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.ExhibitRepository;

public class ExhibitRepository : IExhibitRepository
{
    private readonly JsonFileDatabase<Exhibit> _exhibitDatabase;
    private readonly JsonFileDatabase<ExhibitVariation> _exhibitVariationDatabase;

    public ExhibitRepository(JsonFileDatabase<Exhibit> exhibitDatabase, JsonFileDatabase<ExhibitVariation> exhibitVariationDatabase)
    {
        _exhibitDatabase = exhibitDatabase;
        _exhibitVariationDatabase = exhibitVariationDatabase;
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

    public async Task<List<ExhibitVariation>?> GetAllExhibitVariations()
    {
        var exhibitVariations = await _exhibitVariationDatabase.ReadDataAsync();
        return exhibitVariations;
    }

    public async Task<List<Exhibit>?> GetAllExhibits()
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