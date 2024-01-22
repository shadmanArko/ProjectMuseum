using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Repositories;

public interface IInventoryRepository
{
    Task<List<Equipable>?> GetAllEquipables();
    Task<List<Artifact>?> GetAllArtifacts();
    Task<Artifact> AddArtifact(Artifact artifact);
    Task RemoveAllArtifacts();
    Task<Inventory?> GetInventory();
    Task<int> GetNextEmptySlot();
    Task ReleaseOccupiedSlot(int slot);
    Task<Equipable?> AddEquipable(string equipmentType, string subCategory, string smallPngPath);
    Task<Equipable> RemoveEquipable(string equipableId);
    
}