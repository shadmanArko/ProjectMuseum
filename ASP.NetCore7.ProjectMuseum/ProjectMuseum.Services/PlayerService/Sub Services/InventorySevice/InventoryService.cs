using System.Reflection;
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

    //TODO: Find and Create class 
    public async Task<InventoryItem?> SendItemFromInventoryToMine(string inventoryItemId)
    {
        var inventoryItem = await _inventoryRepository.RemoveInventoryItem(inventoryItemId);
        object mineItem = CreateInstanceByName(inventoryItem.Variant);
        Console.WriteLine($"mineItem class: ");
        return new InventoryItem();
    }
    
    private static Type CreateInstanceByName(string variant)
    {
        var assemblyString = Assembly.CreateQualifiedName("ProjectMuseum.Models", variant);
        var assembly = Assembly.Load(assemblyString);
        return assembly.GetType(variant)!;
    }
    
}