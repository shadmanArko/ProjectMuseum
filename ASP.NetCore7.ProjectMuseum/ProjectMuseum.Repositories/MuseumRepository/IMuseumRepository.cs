using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository;

public interface IMuseumRepository
{
    Task<Museum> Insert(Museum museum);
    Task<Museum> Update(string id, Museum museum);
    Task<Museum?> GetById(string id);
    Task<List<Museum>?> GetAll();
    Task<int> GetMuseumBalance(string id);
    Task<Museum?> Delete(string id);
}