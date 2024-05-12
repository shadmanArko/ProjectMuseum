using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ConsumableRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.ConsumableService;

public class ConsumableService : IConsumableService
{
    private readonly IConsumableRepository _consumableRepository;

    public ConsumableService(IConsumableRepository consumableRepository)
    {
        _consumableRepository = consumableRepository;
    }

    public Task<Consumable> GetConsumableByVariant(string variant)
    {
        var consumable = _consumableRepository.GetConsumableByVariant(variant);
        return consumable;
    }

    public async Task<List<Consumable>> GetAllConsumables()
    {
        var consumables = await _consumableRepository.GetAllConsumables();
        return consumables;
    }
}