using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ResourceRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.ResourceService;

public class ResourceService : IResourceService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly IMineRepository _mineRepository;

    public ResourceService(IResourceRepository resourceRepository, IInventoryRepository inventoryRepository, IMineRepository mineRepository)
    {
        _resourceRepository = resourceRepository;
        _inventoryRepository = inventoryRepository;
        _mineRepository = mineRepository;
    }

    public async Task<InventoryItem?> SendResourceFromMineToInventory(string resourceId)
    {
        var resource = await _resourceRepository.RemoveResourceFromMine(resourceId);
        var inventoryItem = await _inventoryRepository.AddInventoryItem("Resource", resource!.Variant, resource.PNGPath);
        return inventoryItem;
    }

    public async Task AssignResourcesToMine()
    {
        var mine = await _mineRepository.Get();
        var resources = mine.Resources;
        foreach (Cell cell in mine.Cells)
        {
            cell.HasResource = false;
        }

        foreach (var resource in resources)
        {
            var cell = mine.Cells.FirstOrDefault(cell1 =>
                cell1.PositionX == resource.PositionX && cell1.PositionY == resource.PositionY);
            if (cell != null) cell.HasResource = true;
        }
    }
}