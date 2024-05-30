using ProjectMuseum.Models.MIne.Equipables;

namespace ProjectMuseum.Services.MineService.Sub_Services.EquipableService;

public interface IEquipableService
{
    Task<EquipableMelee> GetMeleeByVariant(string variant);
    Task<EquipableRange> GetRangeByVariant(string variant);
    Task<EquipablePickaxe> GetPickaxeByVariant(string variant);
    Task<List<EquipableMelee>> GetAllMeleeEquipables();
    Task<List<EquipableRange>> GetAllRangedEquipables();
    Task<List<EquipablePickaxe>> GetAllPickaxeEquipables();
}