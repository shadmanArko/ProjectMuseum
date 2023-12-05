using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.MineCellCrackService;

public interface IMineCellCrackMaterialService
{
    Task<CellCrackMaterial?> GetCellCrackMaterial(string materialType);

    Task<List<CellCrackMaterial>?> GetAllCellCrackMaterials();
}