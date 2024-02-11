using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.MuseumZoneService;

public interface IMuseumZoneService
{
    Task<MuseumZone> Insert(MuseumZone museumZone);
    Task<MuseumZone> Update(string id, MuseumZone museumZone);
    Task<MuseumZone?> GetById(string id);
    Task<List<MuseumZone>?> GetAll();
    Task<MuseumZone?> Delete(string id);
}