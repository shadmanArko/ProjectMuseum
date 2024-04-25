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
        var listOfEquippableMelee = await _equippableMeleeDatabase.ReadDataAsync();
        return listOfEquippableMelee.FirstOrDefault(melee => melee.Variant == variant);
    }

    public async Task<EquippableRange> GetRangeByVariant(string variant)
    {
        var listOfEquippableRange = await _equippableRangeDatabase.ReadDataAsync();
        return listOfEquippableRange.FirstOrDefault(range => range.Variant == variant);
    }

    public async Task<EquippablePickaxe> GetPickaxeByVariant(string variant)
    {
        var listOfEquippablePickaxe = await _equippablePickaxeDatabase.ReadDataAsync();
        return listOfEquippablePickaxe.FirstOrDefault(pickaxe => pickaxe.Variant == variant);
    }
}