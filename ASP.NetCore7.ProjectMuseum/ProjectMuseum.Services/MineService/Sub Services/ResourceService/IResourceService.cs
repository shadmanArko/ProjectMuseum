using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;

namespace ProjectMuseum.Services.MineService.Sub_Services.ResourceService;

public interface IResourceService
{
    Task<InventoryItem?> SendResourceFromMineToInventory(string resourceId);
    Task<Mine> AssignResourcesToMine();
    Task<List<Resource>> GenerateResources();
}