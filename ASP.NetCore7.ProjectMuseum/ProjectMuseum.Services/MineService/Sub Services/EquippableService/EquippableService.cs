using ProjectMuseum.Models.MIne.Equippables;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.EquipableRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.EquippableService;

public class EquippableService : IEquippableService
{
    private readonly IEquippableRepository _equippableRepository;

    public EquippableService(IEquippableRepository equippableRepository)
    {
        _equippableRepository = equippableRepository;
    }

    public async Task<EquippableMelee> GetMeleeByVariant(string variant)
    {
       return await _equippableRepository.GetMeleeByVariant(variant);
    }

    public async Task<EquippableRange> GetRangeByVariant(string variant)
    {
        return await _equippableRepository.GetRangeByVariant(variant);
    }

    public async Task<EquippablePickaxe> GetPickaxeByVariant(string variant)
    {
        return await _equippableRepository.GetPickaxeByVariant(variant);
    }
}