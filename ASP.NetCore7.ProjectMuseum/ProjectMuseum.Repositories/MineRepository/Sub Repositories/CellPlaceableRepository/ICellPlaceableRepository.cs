using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CellPlaceableRepository;

public interface ICellPlaceableRepository
{
    Task<List<CellPlaceable>> GetAllCellPlaceables();
}