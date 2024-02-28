using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository;

public interface IMuseumRepository
{
    Task<Museum> Insert(Museum museum);
    Task<Museum?> GetById(string id);
    Task<List<Museum>?> GetAll();
    Task<MuseumTicketCounter> UpdateMuseumTicketCounter(MuseumTicketCounter museumTicketCounter);
    Task<float> GetMuseumBalance(string id);
    Task<Museum> ReduceMuseumBalance(string id, float amount);
    Task<Museum> AddToMuseumBalance(string id, float amount);
    Task<Museum?> Delete(string id);
}