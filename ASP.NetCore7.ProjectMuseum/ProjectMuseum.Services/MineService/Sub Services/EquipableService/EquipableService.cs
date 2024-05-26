using ProjectMuseum.Models.MIne.Equipables;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.EquipableRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.EquipableService;

public class EquipableService : IEquipableService
{
    private readonly IEquipableRepository _equipableRepository;

    public EquipableService(IEquipableRepository equipableRepository)
    {
        _equipableRepository = equipableRepository;
    }

    public async Task<EquipableMelee> GetMeleeByVariant(string variant)
    {
       return await _equipableRepository.GetMeleeByVariant(variant);
    }

    public async Task<EquipableRange> GetRangeByVariant(string variant)
    {
        return await _equipableRepository.GetRangeByVariant(variant);
    }

    public async Task<EquipablePickaxe> GetPickaxeByVariant(string variant)
    {
        return await _equipableRepository.GetPickaxeByVariant(variant);
    }

    public async Task<List<EquipableMelee>> GetAllMeleeEquipables()
    {
        var listOfMeleeEquipables = await _equipableRepository.GetAllMeleeEquipables();
        return listOfMeleeEquipables;
    }

    public async Task<List<EquipableRange>> GetAllRangedEquipables()
    {
        var listOfRangedEquipables = await _equipableRepository.GetAllRangedEquipables();
        return listOfRangedEquipables;
    }

    public async Task<List<EquipablePickaxe>> GetAllPickaxeEquipables()
    {
        var listOfPickaxeEquipables = await _equipableRepository.GetAllPickaxeEquipables();
        return listOfPickaxeEquipables;
    }
}