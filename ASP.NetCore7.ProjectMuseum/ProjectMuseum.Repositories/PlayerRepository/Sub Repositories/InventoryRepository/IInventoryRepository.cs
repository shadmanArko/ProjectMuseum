using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories;

public interface IInventoryRepository
{
    Task<List<InventoryItem>?> GetAllEquipables();
    Task<List<Artifact>?> GetAllArtifacts();
    Task<Artifact> AddArtifact(Artifact artifact);
    Task RemoveAllArtifacts();
    Task<Inventory?> GetInventory();
    Task<List<int>?> GetEmptySlots();
    Task<int> GetNextEmptySlot();
    Task ReleaseOccupiedSlot(int slot);
    Task<InventoryItem> AddInventoryItem(string type, string variant, string pngPath);
    Task<InventoryItem> RemoveInventoryItem(string inventoryItemId);
}