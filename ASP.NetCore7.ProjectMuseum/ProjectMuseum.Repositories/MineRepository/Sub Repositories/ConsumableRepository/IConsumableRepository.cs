using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ConsumableRepository;

public interface IConsumableRepository
{
    Task<Consumable> GetConsumableByVariant(string variant);
    Task<List<Consumable>> GetAllConsumables();
}