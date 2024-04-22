using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.ConsumableService;

public interface IConsumableService
{
    Task<Consumable> GetConsumableByVariant(string variant);
}