using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineCellCrackMaterialRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.MineCellCrackService;

public class MineCellCrackMaterialService : IMineCellCrackMaterialService
{
    private readonly IMineCellCrackMaterialRepository _mineCellCrackMaterialRepository;

    public MineCellCrackMaterialService(IMineCellCrackMaterialRepository mineCellCrackMaterialRepository)
    {
        _mineCellCrackMaterialRepository = mineCellCrackMaterialRepository;
    }
    public async Task<CellCrackMaterial?> GetCellCrackMaterial(string materialType)
    {
        var cellCrackMaterial = await _mineCellCrackMaterialRepository.GetCellCrackMaterial(materialType);
        return cellCrackMaterial;
    }

    public async Task<List<CellCrackMaterial>?> GetAllCellCrackMaterials()
    {
        return await _mineCellCrackMaterialRepository.GetAllCellCrackMaterials();
    }
}