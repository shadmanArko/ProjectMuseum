using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.MuseumZoneRepository;

public class MuseumZoneRepository : IMuseumZoneRepository
{
    private readonly JsonFileDatabase<Museum> _museumDatabase;
    
    public MuseumZoneRepository(JsonFileDatabase<Museum> museumDatabase)
    {
        _museumDatabase = museumDatabase;
    }
    public async Task<MuseumZone> Insert(MuseumZone museumZone)
    {
        var museums = await _museumDatabase.ReadDataAsync();
        museums?[0].MuseumZones?.Add(museumZone);
        if (museums != null) await _museumDatabase.WriteDataAsync(museums);
        return museumZone;
    }

    public async Task<MuseumZone> Update(string id, MuseumZone museumZone)
    {
        var museums = await _museumDatabase.ReadDataAsync();
        museums![0].MuseumZones.RemoveAll(zone => zone.Id == id);
        museumZone.Id = id;
        museums![0].MuseumZones.Add(museumZone);
        await _museumDatabase.WriteDataAsync(museums);
        return museumZone;
    }

    public async Task<MuseumZone?> GetById(string id)
    {
        var museums = await _museumDatabase.ReadDataAsync();
        var museumZone = museums![0].MuseumZones.FirstOrDefault(tile => tile.Id == id);
        return museumZone;
    }

    public async Task<List<MuseumZone>?> GetAll()
    {
        var museums = await _museumDatabase.ReadDataAsync();
        return museums![0].MuseumZones;
    }

    public async Task<MuseumZone?> Delete(string id)
    {
        var museums = await _museumDatabase.ReadDataAsync();
        var museumZone = museums![0].MuseumZones.FirstOrDefault(tile => tile.Id == id);
        if (museumZone != null) museums![0].MuseumZones.Remove(museumZone);
        await _museumDatabase.WriteDataAsync(museums);
        return museumZone;
    }
}