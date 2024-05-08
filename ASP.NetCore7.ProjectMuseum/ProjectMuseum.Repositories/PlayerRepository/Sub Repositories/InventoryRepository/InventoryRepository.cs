using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactDescriptiveRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactFunctionalRepository;

namespace ProjectMuseum.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly JsonFileDatabase<Inventory> _inventoryDatabase;
    private readonly JsonFileDatabase<Resource> _resourceDatabase;
    private readonly IRawArtifactFunctionalRepository _rawArtifactFunctionalRepository;
    private readonly IRawArtifactDescriptiveRepository _rawArtifactDescriptiveRepository;
    

    public InventoryRepository(JsonFileDatabase<Inventory> inventoryDatabase, JsonFileDatabase<Resource> resourceDatabase, IRawArtifactFunctionalRepository rawArtifactFunctionalRepository, IRawArtifactDescriptiveRepository rawArtifactDescriptiveRepository)
    {
        _inventoryDatabase = inventoryDatabase;
        _resourceDatabase = resourceDatabase;
        _rawArtifactFunctionalRepository = rawArtifactFunctionalRepository;
        _rawArtifactDescriptiveRepository = rawArtifactDescriptiveRepository;
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

        if (artifacts != null)
        {
            foreach (var artifact in artifacts)
            {
                await ReleaseOccupiedSlot(artifact.Slot);
                var inventoryItem = inventory!.InventoryItems.FirstOrDefault(item => item.Id == artifact.Id);
                if(inventoryItem != null) inventory.InventoryItems.Remove(inventoryItem);
            }
        }
        
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
        return inventory?.OccupiedSlots;
    }

    public async Task<int> GetNextEmptySlot()
    {
        var inventories = await _inventoryDatabase.ReadDataAsync();
        var inventory = inventories?[0];
        var occupiedSlots = inventory?.OccupiedSlots;
        var allSlots = new List<int>();
        for (int i = 0; i < inventory!.SlotsUnlocked; i++)
        {
            allSlots.Add(i);
        }
        
        var emptySlots = allSlots.Except(occupiedSlots!).ToList();
        if (emptySlots.Count == 0) return -1;   // negative number means inventory is full
        inventory.OccupiedSlots.Add(emptySlots[0]);
        await _inventoryDatabase.WriteDataAsync(inventories!);
        return emptySlots[0];
    }

    public async Task ReleaseOccupiedSlot(int slot)
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var occupiedSlots = inventory?.OccupiedSlots;
        occupiedSlots!.Remove(occupiedSlots.FirstOrDefault(slot1 => slot1 == slot));
        inventory!.OccupiedSlots = occupiedSlots;
        await _inventoryDatabase.WriteDataAsync(listOfInventory!);
    }
    
    public async Task<Artifact> AddArtifact(Artifact artifact)
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var newEmptySlot = await GetNextEmptySlot();

        var rawArtifactFunctionals = await _rawArtifactFunctionalRepository.GetAllRawArtifactFunctional();
        var rawArtifactFunctional = rawArtifactFunctionals!.FirstOrDefault(raw => raw.Id == artifact.RawArtifactId);
        var rawArtifactDescriptives = await _rawArtifactDescriptiveRepository.GetAllRawArtifactDescriptive();
        var rawArtifactDescriptive = rawArtifactDescriptives!.FirstOrDefault(raw => raw.Id == artifact.RawArtifactId);
        
        var inventoryItem = new InventoryItem
        {
            Id = artifact.Id,
            Type = "Artifact",
            Category = "",
            Variant = rawArtifactDescriptive!.ArtifactName,
            IsStackable = false,
            Name = rawArtifactDescriptive.ArtifactName,
            PngPath = rawArtifactFunctional!.SmallImageLocation,
            Slot = newEmptySlot,
            Stack = 1
        };
        
        artifact.Slot = newEmptySlot;
        inventory?.OccupiedSlots.Add(newEmptySlot);
        var artifacts = inventory?.Artifacts;
        var inventoryItems = inventory!.InventoryItems;
        inventoryItems.Add(inventoryItem);
        artifacts?.Add(artifact);
        if (listOfInventory != null) await _inventoryDatabase.WriteDataAsync(listOfInventory);
        return artifact;
    }
    
    public async Task<InventoryItem> AddInventoryItem(string type, string variant, string pngPath)
    {
        var inventories = await _inventoryDatabase.ReadDataAsync();
        var inventory = inventories?[0];
        InventoryItem item;
        
        if (inventory!.InventoryItems.Any(item1 => item1.Variant == variant))
        {
            item = inventory.InventoryItems.FirstOrDefault(item1 => item1.Variant == variant)!;
            item.Stack++;
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
                Variant = variant,
                PngPath = pngPath
            };
            
            inventory.OccupiedSlots.Add(item.Slot);
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