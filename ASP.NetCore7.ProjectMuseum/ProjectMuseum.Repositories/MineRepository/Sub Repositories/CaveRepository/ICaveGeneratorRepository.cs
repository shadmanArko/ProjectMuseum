using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CaveRepository;

public interface ICaveGeneratorRepository
{
    Task<Cave> AddCave(Cave cave);
}