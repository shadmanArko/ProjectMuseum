using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ProceduralMineGenerationRepository;

public interface IProceduralMineGenerationRepository
{
    Task<ProceduralMineGenerationData> GetProceduralMineGenerationData();
}