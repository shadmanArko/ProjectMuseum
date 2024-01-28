using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.ResourceService;

public interface IResourceService
{
    Task<InventoryItem?> SendResourceFromMineToInventory(string resourceId);
    Task<Mine> AssignResourcesToMine();
}