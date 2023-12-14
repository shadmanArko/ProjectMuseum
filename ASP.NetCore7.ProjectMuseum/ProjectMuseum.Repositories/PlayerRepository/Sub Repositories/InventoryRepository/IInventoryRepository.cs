using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories;

public interface IInventoryRepository
{
    Task<List<Weapon>?> GetAllWeapons();
    Task<List<Artifact>?> GetAllArtifacts();
    Task<Artifact> AddArtifact(Artifact artifact);
    Task RemoveAllArtifacts();
}