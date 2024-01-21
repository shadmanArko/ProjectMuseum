using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly JsonFileDatabase<Inventory> _inventoryDatabase;

    public InventoryRepository(JsonFileDatabase<Inventory> inventoryDatabase)
    {
        _inventoryDatabase = inventoryDatabase;
    }
    
    public async Task<List<Equipable>?> GetAllEquipables()
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
        occupiedSlots!.Add(emptySlots[0]);
        await _inventoryDatabase.WriteDataAsync(inventories!);
        return emptySlots[0];
    }

    public async Task ReleaseOccupiedSlot(int slot)
    {
        var listOfInventory = await _inventoryDatabase.ReadDataAsync();
        var inventory = listOfInventory?[0];
        var occupiedSlots = inventory?.OccupiedSlots;
        occupiedSlots!.Remove(occupiedSlots.FirstOrDefault(slot1 => slot1 == slot));
        await _inventoryDatabase.WriteDataAsync(listOfInventory!);
    }

    public async Task<Equipable?> AddEquipable(string equipmentType, string equipmentCategory, string smallPngPath)
    {
        var inventories = await _inventoryDatabase.ReadDataAsync();
        var inventory = inventories?[0];

        if (inventory != null && inventory.Equipables.Any(equipable => equipable.EquipmentType == equipmentType))
        {
            if(inventory.Equipables.Any(equipable => equipable.EquipmentCategory == equipmentCategory))
            {
                var equipable = inventory.Equipables.FirstOrDefault(equipable1 => equipable1.EquipmentCategory == equipmentCategory);
                if (equipable != null)
                {
                    equipable.StackNo++;
                    if (inventories != null) await _inventoryDatabase.WriteDataAsync(inventories);
                    return equipable;
                }
            }
        }

        if (inventory!.OccupiedSlots.Count >= inventory.SlotsUnlocked) return null;
        var newEquipable = await CreateNewEquipable(equipmentType, equipmentCategory, smallPngPath);
        inventory?.Equipables.Add(newEquipable);
        if (inventories != null) await _inventoryDatabase.WriteDataAsync(inventories);
        return newEquipable;
    }

    private async Task<Equipable> CreateNewEquipable(string equipmentType, string equipmentCategory, string smallPngPath)
    {
        return new Equipable
        {
            Id = new Guid().ToString(),
            Name = equipmentCategory,
            Slot = await GetNextEmptySlot(),
            EquipmentType = equipmentType,
            EquipmentCategory = equipmentCategory,
            IsStackable = true,
            SmallPngPath = smallPngPath,
            StackNo = 1
        };
    }

    public async Task<Equipable> RemoveEquipable(string equipableId)
    {
        var inventories = await _inventoryDatabase.ReadDataAsync();
        var inventory = inventories?[0];
        var equipables = inventory?.Equipables;
        var equipable = equipables?.FirstOrDefault(equipable1 => equipable1.Id == equipableId);
        
        equipable!.StackNo--;

        if (equipable.StackNo <= 0)
        {
            equipables?.Remove(equipable);
        }

        if (inventories != null) await _inventoryDatabase.WriteDataAsync(inventories);
        return equipable;
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