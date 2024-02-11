using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.WallPlaceableRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.WallPlaceableService;

public class WallPlaceableService : IWallPlaceableService
{
    private readonly JsonFileDatabase<WallPlaceable> _wallPlaceableDatabase;
    private readonly IWallPlaceableRepository _wallPlaceableRepository;

    public WallPlaceableService(JsonFileDatabase<WallPlaceable> wallPlaceableDatabase, IWallPlaceableRepository wallPlaceableRepository)
    {
        _wallPlaceableDatabase = wallPlaceableDatabase;
        _wallPlaceableRepository = wallPlaceableRepository;
    }

    public async Task<WallPlaceable> PlaceWallPlaceableInMine(string variant, List<string> cellIds)
    {
        var wallPlaceables = await _wallPlaceableDatabase.ReadDataAsync();
        var wallPlaceable = wallPlaceables?.FirstOrDefault(placeable => placeable.Variant == variant);
        
        wallPlaceable!.Id = Guid.NewGuid().ToString();
        wallPlaceable.OccupiedCellIds = cellIds;
        
        var modifiedWallPlaceable = await _wallPlaceableRepository.AddWallPlaceable(wallPlaceable);
        return modifiedWallPlaceable;
    }

    public async Task<WallPlaceable?> RemoveWallPlaceableFromMine(string wallPlaceableId)
    {
        var removedWallPlaceable = await _wallPlaceableRepository.RemoveWallPlaceable(wallPlaceableId);
        return removedWallPlaceable;
    }
}