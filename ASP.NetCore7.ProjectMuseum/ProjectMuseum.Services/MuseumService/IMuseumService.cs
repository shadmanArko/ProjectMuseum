using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService;

public interface IMuseumService
{
    Task<int> GetMuseumCurrentMoneyAmount(string id);
    Task<Museum> AddToMuseumBalance(string id, int amount);
    Task<Museum> ReduceMuseumBalance(string id, int amount);
}