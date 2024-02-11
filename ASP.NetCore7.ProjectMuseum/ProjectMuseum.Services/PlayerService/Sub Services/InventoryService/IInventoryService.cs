using ProjectMuseum.Models;

namespace ProjectMuseum.Services.PlayerService.Sub_Services.InventoryService;

public interface IInventoryService
{
    Task<List<InventoryItem>?> GetAllEquipables();
    Task<List<Artifact>?> GetAllArtifacts();
    Task SendAllArtifactsToArtifactStorage();
    Task<Inventory?> GetInventory();
    Task<WallPlaceable> SendWallPlaceableFromInventoryToMine(string inventoryItemId, List<string> cellIds);

}