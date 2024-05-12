using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CellPlaceableRepository;

public class CellPlaceableRepository : ICellPlaceableRepository
{
    private readonly JsonFileDatabase<CellPlaceable> _cellPlaceableDatabase;

    public CellPlaceableRepository(JsonFileDatabase<CellPlaceable> cellPlaceableDatabase)
    {
        _cellPlaceableDatabase = cellPlaceableDatabase;
    }

    public async Task<List<CellPlaceable>> GetAllCellPlaceables()
    {
        var cellPlaceables = await _cellPlaceableDatabase.ReadDataAsync();
        return cellPlaceables!;
    }
}