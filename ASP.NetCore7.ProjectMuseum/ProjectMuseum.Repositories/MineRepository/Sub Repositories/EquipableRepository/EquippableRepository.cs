using ProjectMuseum.Models.MIne.Equippables;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.EquipableRepository;

public class EquippableRepository : IEquippableRepository
{
    private readonly JsonFileDatabase<EquippableMelee> _equippableMeleeDatabase; 
    private readonly JsonFileDatabase<EquippableRange> _equippableRangeDatabase; 
    private readonly JsonFileDatabase<EquippablePickaxe> _equippablePickaxeDatabase;

    public EquippableRepository(JsonFileDatabase<EquippableMelee> equippableMeleeDatabase, JsonFileDatabase<EquippableRange> equippableRangeDatabase, JsonFileDatabase<EquippablePickaxe> equippablePickaxeDatabase)
    {
        _equippableMeleeDatabase = equippableMeleeDatabase;
        _equippableRangeDatabase = equippableRangeDatabase;
        _equippablePickaxeDatabase = equippablePickaxeDatabase;
    }

    public async Task<EquippableMelee> GetMeleeByVariant(string variant)
    {
        throw new NotImplementedException();
    }

    public async Task<EquippableRange> GetRangeByVariant(string variant)
    {
        throw new NotImplementedException();
    }

    public async Task<EquippablePickaxe> GetPickaxeByVariant(string variant)
    {
        throw new NotImplementedException();
    }
}