using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumRepository;

namespace ProjectMuseum.Services.MuseumService;

public class MuseumService : IMuseumService
{
    private readonly IMuseumRepository _museumRepository;

    public MuseumService(IMuseumRepository museumRepository)
    {
        _museumRepository = museumRepository;
    }
    public async Task<int> GetMuseumCurrentMoneyAmount(string id)
    {
        return await _museumRepository.GetMuseumBalance(id);
    }

    public async Task<Museum> AddToMuseumBalance(string id, int amount)
    {
        return await _museumRepository.AddToMuseumBalance(id, amount);
    }

    public async Task<Museum> ReduceMuseumBalance(string id, int amount)
    {
        return await _museumRepository.ReduceMuseumBalance(id, amount);
    }
}