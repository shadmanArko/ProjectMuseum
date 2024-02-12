using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.MuseumZoneService;

public interface IMuseumZoneService
{
    Task<MuseumZone> Insert(MuseumZone museumZone);
    Task<MuseumZone> CreateNewZone(MuseumZone museumZone);
    Task<MuseumZone> InsertTilesIntoZone(List<string> tileIds, string zoneId);
    Task<MuseumZone> ReleaseTilesFromZone(List<string> tileIds, string zoneId);
    Task<MuseumZone> EditZone(string id, MuseumZone museumZone);
    Task<MuseumZone?> GetZone(string id);
    Task<List<MuseumZone>?> GetAll();
    Task<MuseumZone?> DeleteZone(string id);
}