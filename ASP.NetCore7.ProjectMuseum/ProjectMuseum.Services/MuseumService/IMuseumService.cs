using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService;

public interface IMuseumService
{
    Task<float> GetMuseumCurrentMoneyAmount(string id);
    Task<Museum> AddToMuseumBalance(string id, float amount);
    Task<Museum> ReduceMuseumBalance(string id, float amount);
}