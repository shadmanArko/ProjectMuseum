using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.WallPlaceableService;

public interface IWallPlaceableService
{
    Task<WallPlaceable> PlaceWallPlaceableInMine(string variant, List<string> cellIds);
    Task<WallPlaceable?> RemoveWallPlaceableFromMine(string wallPlaceableId);
}