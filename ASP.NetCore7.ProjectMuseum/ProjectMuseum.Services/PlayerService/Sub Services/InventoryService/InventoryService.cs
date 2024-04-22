using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;
using ProjectMuseum.Services.MineService.Sub_Services.ConsumableService;
using ProjectMuseum.Services.MineService.Sub_Services.WallPlaceableService;

namespace ProjectMuseum.Services.PlayerService.Sub_Services.InventoryService;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IArtifactStorageRepository _artifactStorageRepository;
    private readonly IWallPlaceableService _wallPlaceableService;
    private readonly IConsumableService _consumableService;

    public InventoryService(IInventoryRepository inventoryRepository, IArtifactStorageRepository artifactStorageRepository, IWallPlaceableService wallPlaceableService, IConsumableService consumableService)
    {
        _inventoryRepository = inventoryRepository;
        _artifactStorageRepository = artifactStorageRepository;
        _wallPlaceableService = wallPlaceableService;
        _consumableService = consumableService;
    }

    public async Task<List<InventoryItem>?> GetAllEquipables()
    {
        var equipables = await _inventoryRepository.GetAllEquipables();
        return equipables;
    }

    public async Task<List<Artifact>?> GetAllArtifacts()
    {
        var artifacts = await _inventoryRepository.GetAllArtifacts();
        return artifacts;
    }
    
    public async Task SendAllArtifactsToArtifactStorage()
    {
        var artifacts = await _inventoryRepository.GetAllArtifacts();
        await _artifactStorageRepository.AddListOfArtifacts(artifacts!);
        await _inventoryRepository.RemoveAllArtifacts();
    }

    public async Task<Inventory?> GetInventory()
    {
        var inventory = await _inventoryRepository.GetInventory();
        return inventory;
    }

    public async Task<WallPlaceable> SendWallPlaceableFromInventoryToMine(string inventoryItemId, List<string> cellIds)
    {
        var inventoryItem = await _inventoryRepository.RemoveInventoryItem(inventoryItemId);
        var wallPlaceable = await _wallPlaceableService.PlaceWallPlaceableInMine(inventoryItem.Variant, cellIds);

        return wallPlaceable;
    }

    public async Task<Consumable> SendConsumableFromInventoryToMine(string inventoryItemId)
    {
        var inventoryItem = await _inventoryRepository.RemoveInventoryItem(inventoryItemId);
        var consumable = await _consumableService.GetConsumableByVariant(inventoryItem.Variant);
        return consumable;
    }
}