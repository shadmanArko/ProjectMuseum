using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.MuseumZoneRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.MuseumZoneService;

public class MuseumZoneService : IMuseumZoneService
{
    private readonly IMuseumZoneRepository _museumZoneRepository;
    private readonly IMuseumTileRepository _museumTileRepository;

    public MuseumZoneService(IMuseumZoneRepository museumZoneRepository, IMuseumTileRepository museumTileRepository)
    {
        _museumZoneRepository = museumZoneRepository;
        _museumTileRepository = museumTileRepository;
    }

    public async Task<MuseumZone> Insert(MuseumZone museumZone)
    {
        museumZone = await _museumZoneRepository.Insert(museumZone);
        return museumZone;
    }

    public async Task<MuseumZone> CreateNewZone(MuseumZone museumZone)
    {
        museumZone = await _museumZoneRepository.Insert(museumZone);
        return museumZone;
    }

    public async Task<MuseumZone> InsertTilesIntoZone(List<string> tileIds, string zoneId)
    {
        
        await SetIsInZone(tileIds, true);
        foreach (var museumZone in (await _museumZoneRepository.GetAll())!)
        {
            await ReleaseTilesFromZone(tileIds, museumZone.Id);
        }
        var zone = await GetZone(zoneId);
        if (zone != null) zone.OccupiedMuseumTileIds = (List<string>)zone.OccupiedMuseumTileIds.Concat(tileIds);
        zone = await EditZone(zoneId, zone);
        return zone;
    }

    private async Task SetIsInZone(List<string> tileIds, bool isInZone)
    {
        foreach (var tileId in tileIds)
        {
            var tile = await _museumTileRepository.GetById(tileId);
            tile!.IsInZone = isInZone;
            await _museumTileRepository.Update(tileId, tile);
        }
    }

    public async Task<MuseumZone> ReleaseTilesFromZone(List<string> tileIds, string zoneId)
    {
        await SetIsInZone(tileIds, false);
        var zone = await GetZone(zoneId);
        if (zone != null) zone.OccupiedMuseumTileIds = new List<string>(zone.OccupiedMuseumTileIds.RemoveAll(tileIds.Contains));
        zone = await EditZone(zoneId, zone);
        return zone;
    }
    
    public async Task<MuseumZone> EditZone(string id, MuseumZone museumZone)
    {
        museumZone = await _museumZoneRepository.Update(id, museumZone);
        return museumZone;
    }

    public async Task<MuseumZone?> GetZone(string id)
    {
        var museumZone = await _museumZoneRepository.GetById(id);
        return museumZone;
    }

    public async Task<List<MuseumZone>?> GetAll()
    {
        var museumZones = await _museumZoneRepository.GetAll();
        return museumZones;
    }

    public async Task<MuseumZone?> DeleteZone(string id)
    {
        var museumZone = await _museumZoneRepository.Delete(id);
        return museumZone;
    }
}