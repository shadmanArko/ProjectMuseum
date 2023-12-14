using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;

namespace ProjectMuseum.Services.InventorySevice;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IArtifactStorageRepository _artifactStorageRepository;

    public InventoryService(IInventoryRepository inventoryRepository, IArtifactStorageRepository artifactStorageRepository)
    {
        _inventoryRepository = inventoryRepository;
        _artifactStorageRepository = artifactStorageRepository;
    }

    public async Task<List<Weapon>?> GetAllWeapons()
    {
        var weapons = await _inventoryRepository.GetAllWeapons();
        return weapons;
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
}