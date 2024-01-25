using ProjectMuseum.Models;

namespace ProjectMuseum.Services.InventorySevice;

public interface IInventoryService
{
    Task<List<InventoryItem>?> GetAllEquipables();
    Task<List<Artifact>?> GetAllArtifacts();
    Task SendAllArtifactsToArtifactStorage();
    Task<Inventory?> GetInventory();
    Task<InventoryItem?> SendItemFromInventoryToMine(string inventoryItemId);
}