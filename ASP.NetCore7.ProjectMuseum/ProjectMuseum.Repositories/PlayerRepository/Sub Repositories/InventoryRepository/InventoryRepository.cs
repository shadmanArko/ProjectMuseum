using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly JsonFileDatabase<Inventory> _inventoryDatabase;

    public InventoryRepository(JsonFileDatabase<Inventory> inventoryDatabase)
    {
        _inventoryDatabase = inventoryDatabase;
    }
    
    public async Task<List<InventoryItem>?> GetAllEquipables()
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var equipables = inventory?.Equipables;
        return equipables;
    }

    public async Task<List<Artifact>?> GetAllArtifacts()
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var artifacts = inventory?.Artifacts;
        return artifacts;
    }

    public async Task RemoveAllArtifacts()
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var artifacts = inventory?.Artifacts;
        artifacts?.Clear();
        if (listOfInventory != null) await _inventoryDatabase.WriteDataAsync(listOfInventory);
    }

    public async Task<Inventory?> GetInventory()
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        return inventory;
    }

    public async Task<List<int>?> GetEmptySlots()
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        return inventory?.EmptySlots;
    }

    public async Task<int> GetNextEmptySlot()
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var emptySlots = inventory?.EmptySlots;
        //TODO: check for empty slot and return most recent one
        return emptySlots![0];
    }

    public async Task ReleaseOccupiedSlot(int slot)
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var emptySlots = inventory?.EmptySlots;
        emptySlots?.RemoveAt(slot);
        var tempSlots = new List<int>();
        if (emptySlots != null)
        {
            foreach (var emptySlot in emptySlots)
            {
                if (emptySlot == 0) continue;
                tempSlots.Add(emptySlot);
            }
        }

        inventory!.EmptySlots = tempSlots;
    }

    public async Task<Artifact> AddArtifact(Artifact artifact)
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var artifacts = inventory?.Artifacts;
        artifacts?.Add(artifact);
        if (listOfInventory != null) await _inventoryDatabase.WriteDataAsync(listOfInventory);
        return artifact;
    }
}