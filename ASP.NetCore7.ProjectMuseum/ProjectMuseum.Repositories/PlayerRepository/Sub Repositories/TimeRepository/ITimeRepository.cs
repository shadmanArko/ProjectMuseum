using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.PlayerRepository.Sub_Repositories.TimeRepository;

public interface ITimeRepository
{
    Task<Time?> Get();
    Task<Time?> Update(Time time);
}