using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.CellPlaceableService;

public interface ICellPlaceableService
{
    Task<List<CellPlaceable>> GetAllCellPlaceables();
}