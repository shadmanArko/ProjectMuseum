using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;

namespace ProjectMuseum.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly JsonFileDatabase<Inventory> _inventoryDatabase;
    private readonly JsonFileDatabase<Resource> _resourceDatabase;

    public InventoryRepository(JsonFileDatabase<Inventory> inventoryDatabase, JsonFileDatabase<Resource> resourceDatabase)
    {
        _inventoryDatabase = inventoryDatabase;
        _resourceDatabase = resourceDatabase;
    }
    
    public async Task<List<InventoryItem>?> GetAllEquipables()
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var equipables = inventory?.InventoryItems;
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
    
    public async Task<InventoryItem> AddInventoryItem(string type, string variant)
    {
        var inventories = await _inventoryDatabase.ReadDataAsync();
        var inventory = inventories?[0];
        var item = new InventoryItem();
        
        if (inventory!.InventoryItems.Any(item1 => item1.Variant == variant))
        {
            item = inventory.InventoryItems.FirstOrDefault(item1 => item1.Variant == variant);
            item!.Stack++;
        }
        else
        {
            item = new InventoryItem
            {
                Id = Guid.NewGuid().ToString(),
                IsStackable = true,
                Name = variant,
                Stack = 1,
                Slot = await GetNextEmptySlot(),
                Type = type,
                Variant = variant
            };
            
            inventory.InventoryItems.Add(item);
        }
        Console.WriteLine($"item id: {item.Id}, item variant: {item.Variant}");
        
        await _inventoryDatabase.WriteDataAsync(inventories!);
        return item;
    }

    public async Task<InventoryItem> RemoveInventoryItem(string inventoryItemId)
    {
        var inventories = await _inventoryDatabase.ReadDataAsync();
        var inventory = inventories?[0];
        var inventoryItemToRemove = inventory?.InventoryItems.FirstOrDefault(item1 => item1.Id == inventoryItemId);
        
        if (inventoryItemToRemove!.IsStackable && inventoryItemToRemove.Stack > 1)
            inventoryItemToRemove.Stack--;
        else 
            inventory?.InventoryItems.Remove(inventoryItemToRemove);

        await _inventoryDatabase.WriteDataAsync(inventories!);
        return inventoryItemToRemove;
    }
}