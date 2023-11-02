using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository;

public interface IMineRepository
{
    Task<Mine> Get();
}