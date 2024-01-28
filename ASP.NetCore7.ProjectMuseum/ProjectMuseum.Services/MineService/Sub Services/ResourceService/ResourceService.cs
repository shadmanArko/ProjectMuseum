using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ResourceRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.ResourceService;

public class ResourceService : IResourceService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IResourceRepository _resourceRepository;

    public ResourceService(IResourceRepository resourceRepository, IInventoryRepository inventoryRepository)
    {
        _resourceRepository = resourceRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<InventoryItem?> SendResourceFromMineToInventory(string resourceId)
    {
        var resource = await _resourceRepository.RemoveResourceFromMine(resourceId);
        var inventoryItem = await _inventoryRepository.AddInventoryItem("Resource", resource!.Variant);
        return inventoryItem;
    }
}