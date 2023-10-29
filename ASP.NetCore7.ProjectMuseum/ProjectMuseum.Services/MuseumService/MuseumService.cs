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
}