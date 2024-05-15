using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ConsumableRepository;

public class ConsumableRepository : IConsumableRepository
{
    private readonly JsonFileDatabase<Consumable> _consumableDatabase;

    public ConsumableRepository(JsonFileDatabase<Consumable> consumableDatabase)
    {
        _consumableDatabase = consumableDatabase;
    }
    
    public async Task<Consumable> GetConsumableByVariant(string variant)
    {
        var consumables = await _consumableDatabase.ReadDataAsync();
        var consumable = consumables!.FirstOrDefault(temp => temp.Variant == variant);
        return consumable!;
    }

    public async Task<List<Consumable>> GetAllConsumables()
    {
        var consumables = await _consumableDatabase.ReadDataAsync();
        return consumables!;
    }
}