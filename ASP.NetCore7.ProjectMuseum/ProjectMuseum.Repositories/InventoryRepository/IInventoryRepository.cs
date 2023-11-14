using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories;

public interface IInventoryRepository
{
    Task<List<Weapon>?> GetAllWeapons();
    Task<List<Artifact>?> GetAllArtifacts();
}