using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.MuseumZoneRepository;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.MuseumZoneService;

public class MuseumZoneService : IMuseumZoneService
{
    private readonly IMuseumZoneRepository _museumZoneRepository;

    public MuseumZoneService(IMuseumZoneRepository museumZoneRepository)
    {
        _museumZoneRepository = museumZoneRepository;
    }

    public async Task<MuseumZone> Insert(MuseumZone museumZone)
    {
        museumZone = await _museumZoneRepository.Insert(museumZone);
        return museumZone;
    }

    public async Task<MuseumZone> Update(string id, MuseumZone museumZone)
    {
        museumZone = await _museumZoneRepository.Update(id, museumZone);
        return museumZone;
    }

    public async Task<MuseumZone?> GetById(string id)
    {
        var museumZone = await _museumZoneRepository.GetById(id);
        return museumZone;
    }

    public async Task<List<MuseumZone>?> GetAll()
    {
        var museumZones = await _museumZoneRepository.GetAll();
        return museumZones;
    }

    public async Task<MuseumZone?> Delete(string id)
    {
        var museumZone = await _museumZoneRepository.Delete(id);
        return museumZone;
    }
}