using ProjectMuseum.Models.MIne.Equippables;

namespace ProjectMuseum.Services.MineService.Sub_Services.EquippableService;

public interface IEquippableService
{
    Task<EquippableMelee> GetMeleeByVariant(string variant);
    Task<EquippableRange> GetRangeByVariant(string variant);
    Task<EquippablePickaxe> GetPickaxeByVariant(string variant);
}