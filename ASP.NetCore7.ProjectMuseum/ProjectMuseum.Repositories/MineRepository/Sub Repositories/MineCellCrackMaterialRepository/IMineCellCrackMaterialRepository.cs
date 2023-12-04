using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineCellCrackMaterialRepository;

public interface IMineCellCrackMaterialRepository
{
    Task<CellCrackMaterial?> GetCellCrackMaterial(string materialType);
    Task<List<CellCrackMaterial>?> GetAllCellCrackMaterials();
}