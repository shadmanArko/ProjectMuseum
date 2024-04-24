using ProjectMuseum.Models.MIne.Equippables;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.EquipableRepository;

public interface IEquippableRepository
{
    Task<EquippableMelee> GetMeleeByVariant(string variant);
    Task<EquippableRange> GetRangeByVariant(string variant);
    Task<EquippablePickaxe> GetPickaxeByVariant(string variant);
}