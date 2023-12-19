using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories;

public interface IInventoryRepository
{
    Task<List<Equipable>?> GetAllEquipables();
    Task<List<Artifact>?> GetAllArtifacts();
    Task<Artifact> AddArtifact(Artifact artifact);
    Task RemoveAllArtifacts();
    Task<Inventory?> GetInventory();
}