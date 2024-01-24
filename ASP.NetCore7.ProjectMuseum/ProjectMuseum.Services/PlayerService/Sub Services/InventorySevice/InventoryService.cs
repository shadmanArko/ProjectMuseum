using ProjectMuseum.Models;
using ProjectMuseum.Models.TransportChainBlocks;
using ProjectMuseum.Models.Vehicles;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.TransportChainBlockRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;

namespace ProjectMuseum.Services.InventorySevice;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IArtifactStorageRepository _artifactStorageRepository;
    private readonly IMineVehicleRepository _mineVehicleRepository;
    private readonly IMineTransportChainBlockRepository _mineTransportChainBlockRepository;

    public InventoryService(IInventoryRepository inventoryRepository, IArtifactStorageRepository artifactStorageRepository, IMineVehicleRepository mineVehicleRepository, IMineTransportChainBlockRepository mineTransportChainBlockRepository)
    {
        _inventoryRepository = inventoryRepository;
        _artifactStorageRepository = artifactStorageRepository;
        _mineVehicleRepository = mineVehicleRepository;
        _mineTransportChainBlockRepository = mineTransportChainBlockRepository;
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

    public async Task<Item> SendVehicleToMine(string equipableId)
    {
        var equipable = await _inventoryRepository.RemoveEquipable(equipableId);
        await _inventoryRepository.ReleaseOccupiedSlot(equipable.Slot);
        var vehicle = await _mineVehicleRepository.AddVehicleToMine(equipable.ItemSubcategory);
        return vehicle;
    }
    
    public async Task<TransportChainBlock> SendTransportChainBlockToMine(string equipableId)
    {
        var equipable = await _inventoryRepository.RemoveEquipable(equipableId);
        await _inventoryRepository.ReleaseOccupiedSlot(equipable.Slot);
        var chainBlock = await _mineTransportChainBlockRepository.AddTransportChainBlockToMine(equipable.ItemSubcategory);
        return chainBlock;
    }
}