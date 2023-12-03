using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineCellCrackMaterialRepository;

public class MineCellCrackMaterialRepository : IMineCellCrackMaterialRepository
{
    private readonly JsonFileDatabase<CellCrackMaterial> _cellCrackMaterialDatabase;

    public MineCellCrackMaterialRepository(JsonFileDatabase<CellCrackMaterial> cellCrackMaterialDatabase)
    {
        _cellCrackMaterialDatabase = cellCrackMaterialDatabase;
    }

    public async Task<CellCrackMaterial?> GetCellCrackMaterial(string materialType)
    {
        var listOfCellCrackMaterials = await _cellCrackMaterialDatabase.ReadDataAsync();
        var cellCrackMaterial = listOfCellCrackMaterials.FirstOrDefault(material => material.MaterialType == materialType);

        return cellCrackMaterial;
    }
}