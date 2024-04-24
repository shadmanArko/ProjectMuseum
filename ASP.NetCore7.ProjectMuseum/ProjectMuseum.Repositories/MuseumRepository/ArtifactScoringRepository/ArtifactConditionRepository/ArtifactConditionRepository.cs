using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository;

public class ArtifactConditionRepository : IArtifactConditionRepository
{
    private readonly JsonFileDatabase<ArtifactCondition> _artifactConditionDataBase;

    public ArtifactConditionRepository(JsonFileDatabase<ArtifactCondition> artifactConditionDataBase)
    {
        _artifactConditionDataBase = artifactConditionDataBase;
    }
    public async Task<float> GetConditionValueByCondition(string condition)
    {
        var artifactConditions = await _artifactConditionDataBase.ReadDataAsync();
        var artifactCondition = artifactConditions.FirstOrDefault(condition1 => condition1.Condition == condition);
        return artifactCondition.ScoreMultiplier;
    }

    public async Task<List<ArtifactCondition>> GetAllArtifactConditions()
    {
        var artifactConditions = await _artifactConditionDataBase.ReadDataAsync();
        return artifactConditions!;
    }
}