using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;

namespace ProjectMuseum.Services.InventorySevice;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IArtifactStorageRepository _artifactStorageRepository;
    private readonly IMineVehicleRepository _mineVehicleRepository;

    public InventoryService(IInventoryRepository inventoryRepository, IArtifactStorageRepository artifactStorageRepository, IMineVehicleRepository mineVehicleRepository)
    {
        _inventoryRepository = inventoryRepository;
        _artifactStorageRepository = artifactStorageRepository;
        _mineVehicleRepository = mineVehicleRepository;
    }

    public async Task<List<Equipable>?> GetAllEquipables()
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

    public async Task<Vehicle> SendVehicleToMine(string equipableId)
    {
        var equipable = await _inventoryRepository.RemoveEquipable(equipableId);
        await _inventoryRepository.ReleaseOccupiedSlot(equipable.Slot);
        var vehicle = await _mineVehicleRepository.AddVehicleToMine(equipable.EquipmentSubcategory);
        return vehicle;
    }
}