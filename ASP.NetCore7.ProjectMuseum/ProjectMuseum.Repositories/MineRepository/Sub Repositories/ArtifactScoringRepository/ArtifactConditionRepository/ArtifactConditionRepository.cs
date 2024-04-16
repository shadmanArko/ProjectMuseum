using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository;

public class ArtifactConditionRepository : IArtifactConditionRepository
{
    private readonly JsonFileDatabase<ArtifactCondition> _artifactConditionDataBase;

    public ArtifactConditionRepository(JsonFileDatabase<ArtifactCondition> artifactConditionDataBase)
    {
        _artifactConditionDataBase = artifactConditionDataBase;
    }
    public Task<float> GetConditionValueByCondition(string condition)
    {
        throw new NotImplementedException();
    }
}