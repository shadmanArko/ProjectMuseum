using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository;

public interface IMineRepository
{
    Task<Mine> Get();
    Task<Mine> Update(Mine mine);
    
}