using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CaveRepository;

public interface ICaveRepository
{
    Task<Cave> GenerateCave(int xMin, int xMax, int yMin, int yMax);
}