using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository;

public interface IArtifactConditionRepository
{
    Task<float> GetConditionValueByCondition(string condition);
    Task<List<ArtifactCondition>> GetAllArtifactConditions();
}