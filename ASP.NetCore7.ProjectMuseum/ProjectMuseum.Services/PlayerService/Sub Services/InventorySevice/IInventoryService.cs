using ProjectMuseum.Models;

namespace ProjectMuseum.Services.InventorySevice;

public interface IInventoryService
{
    Task<List<Weapon>?> GetAllWeapons();
    Task<List<Artifact>?> GetAllArtifacts();
    Task SendAllArtifactsToArtifactStorage();
}