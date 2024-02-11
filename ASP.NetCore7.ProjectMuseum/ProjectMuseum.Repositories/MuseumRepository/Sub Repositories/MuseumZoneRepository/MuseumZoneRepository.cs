using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.MuseumZoneRepository;

public class MuseumZoneRepository : IMuseumZoneRepository
{
    public Task<MuseumZone> Insert(MuseumZone museumZone)
    {
        throw new NotImplementedException();
    }

    public Task<MuseumZone> Update(string id, MuseumZone museumZone)
    {
        throw new NotImplementedException();
    }

    public Task<MuseumZone?> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<MuseumZone>?> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<MuseumZone?> Delete(string id)
    {
        throw new NotImplementedException();
    }
}