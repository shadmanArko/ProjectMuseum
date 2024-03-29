using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.WallPlaceableRepository;
using ProjectMuseum.Services.PlayerService.Sub_Services.InventoryService;

namespace ProjectMuseum.Services.MineService.Sub_Services.WallPlaceableService;

public class WallPlaceableService : IWallPlaceableService
{
    private readonly JsonFileDatabase<WallPlaceable> _wallPlaceableDatabase;
    private readonly IWallPlaceableRepository _wallPlaceableRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public WallPlaceableService(JsonFileDatabase<WallPlaceable> wallPlaceableDatabase, IWallPlaceableRepository wallPlaceableRepository, IInventoryRepository inventoryRepository)
    {
        _wallPlaceableDatabase = wallPlaceableDatabase;
        _wallPlaceableRepository = wallPlaceableRepository;
        _inventoryRepository = inventoryRepository;
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

    public async Task<InventoryItem?> SendWallPlaceableFromMineToInventory(string wallPlaceableId)
    {
        var removedWallPlaceable = await _wallPlaceableRepository.RemoveWallPlaceable(wallPlaceableId);
        var inventoryItem = await _inventoryRepository.AddInventoryItem(removedWallPlaceable!.Type,
            removedWallPlaceable.Variant, removedWallPlaceable.PngPath);
        return inventoryItem;
    }

    public async Task<WallPlaceable> GetWallPlaceableByVariant(string variant)
    {
        var wallPlaceable = await _wallPlaceableRepository.GetWallPlaceableByVariant(variant);
        return wallPlaceable;
    }
}