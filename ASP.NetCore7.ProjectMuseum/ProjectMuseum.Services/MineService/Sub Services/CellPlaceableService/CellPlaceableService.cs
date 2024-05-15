using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CellPlaceableRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.CellPlaceableService;

public class CellPlaceableService : ICellPlaceableService
{
    private readonly ICellPlaceableRepository _cellPlaceableRepository;

    public CellPlaceableService(ICellPlaceableRepository cellPlaceableRepository)
    {
        _cellPlaceableRepository = cellPlaceableRepository;
    }

    public async Task<List<CellPlaceable>> GetAllCellPlaceables()
    {
        var cellPlaceables = await _cellPlaceableRepository.GetAllCellPlaceables();
        return cellPlaceables;
    }
}