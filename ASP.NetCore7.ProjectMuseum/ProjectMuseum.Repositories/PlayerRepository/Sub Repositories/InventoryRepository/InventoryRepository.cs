using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly JsonFileDatabase<Inventory> _inventoryDatabase;

    public InventoryRepository(JsonFileDatabase<Inventory> inventoryDatabase)
    {
        _inventoryDatabase = inventoryDatabase;
    }
    
    public async Task<List<Weapon>?> GetAllWeapons()
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var weapons = inventory?.Weapons;
        return weapons;
    }

    public async Task<List<Artifact>?> GetAllArtifacts()
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var artifacts = inventory?.Artifacts;
        return artifacts;
    }
}