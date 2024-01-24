using ProjectMuseum.Models;
using ProjectMuseum.Models.TransportChainBlocks;
using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Services.InventorySevice;

public interface IInventoryService
{
    Task<List<Equipable>?> GetAllEquipables();
    Task<List<Artifact>?> GetAllArtifacts();
    Task SendAllArtifactsToArtifactStorage();
    Task<Inventory?> GetInventory();
    Task<Item> SendVehicleToMine(string equipableId);
    Task<TransportChainBlock> SendTransportChainBlockToMine(string equipableId);
}