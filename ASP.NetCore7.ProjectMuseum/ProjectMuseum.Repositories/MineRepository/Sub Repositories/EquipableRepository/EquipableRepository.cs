using ProjectMuseum.Models.MIne.Equipables;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.EquipableRepository;

public class EquipableRepository : IEquipableRepository
{
    private readonly JsonFileDatabase<EquipableMelee> _equipableMeleeDatabase; 
    private readonly JsonFileDatabase<EquipableRange> _equipableRangeDatabase; 
    private readonly JsonFileDatabase<EquipablePickaxe> _equipablePickaxeDatabase;

    public EquipableRepository(JsonFileDatabase<EquipableMelee> equipableMeleeDatabase, JsonFileDatabase<EquipableRange> equipableRangeDatabase, JsonFileDatabase<EquipablePickaxe> equipablePickaxeDatabase)
    {
        _equipableMeleeDatabase = equipableMeleeDatabase;
        _equipableRangeDatabase = equipableRangeDatabase;
        _equipablePickaxeDatabase = equipablePickaxeDatabase;
    }

    public async Task<EquipableMelee> GetMeleeByVariant(string variant)
    {
        var listOfEquipableMelee = await _equipableMeleeDatabase.ReadDataAsync();
        return listOfEquipableMelee.FirstOrDefault(melee => melee.Variant == variant);
    }

    public async Task<EquipableRange> GetRangeByVariant(string variant)
    {
        var listOfEquipableRange = await _equipableRangeDatabase.ReadDataAsync();
        return listOfEquipableRange.FirstOrDefault(range => range.Variant == variant);
    }

    public async Task<EquipablePickaxe> GetPickaxeByVariant(string variant)
    {
        var listOfEquipablePickaxe = await _equipablePickaxeDatabase.ReadDataAsync();
        return listOfEquipablePickaxe.FirstOrDefault(pickaxe => pickaxe.Variant == variant);
    }

    public async Task<List<EquipableMelee>> GetAllMeleeEquipables()
    {
        var listOfMeleeEquipables = await _equipableMeleeDatabase.ReadDataAsync();
        return listOfMeleeEquipables!;
    }

    public async Task<List<EquipableRange>> GetAllRangedEquipables()
    {
        var listOfRangedEquipables = await _equipableRangeDatabase.ReadDataAsync();
        return listOfRangedEquipables!;
    }

    public async Task<List<EquipablePickaxe>> GetAllPickaxeEquipables()
    {
        var listOfPickaxeEquipables = await _equipablePickaxeDatabase.ReadDataAsync();
        return listOfPickaxeEquipables!;
    }
}