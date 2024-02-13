using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.WallPlaceableRepository;

public interface IWallPlaceableRepository
{
    Task<WallPlaceable> AddWallPlaceable(WallPlaceable wallPlaceable);
    Task<WallPlaceable?> RemoveWallPlaceable(string wallPlaceableId);
}