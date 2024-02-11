using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.MuseumZoneRepository;

public interface IMuseumZoneRepository
{
    Task<MuseumZone> Insert(MuseumZone museumZone);
    Task<MuseumZone> Update(string id, MuseumZone museumZone);
    Task<MuseumZone?> GetById(string id);
    Task<List<MuseumZone>?> GetAll();
    Task<MuseumZone?> Delete(string id);
}