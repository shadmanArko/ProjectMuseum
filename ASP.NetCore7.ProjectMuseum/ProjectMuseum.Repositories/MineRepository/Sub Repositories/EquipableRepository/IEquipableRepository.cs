using ProjectMuseum.Models.MIne.Equipables;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.EquipableRepository;

public interface IEquipableRepository
{
    Task<EquipableMelee> GetMeleeByVariant(string variant);
    Task<EquipableRange> GetRangeByVariant(string variant);
    Task<EquipablePickaxe> GetPickaxeByVariant(string variant);
    Task<List<EquipableMelee>> GetAllMeleeEquipables();
    Task<List<EquipableRange>> GetAllRangedEquipables();
    Task<List<EquipablePickaxe>> GetAllPickaxeEquipables();
}