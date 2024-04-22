using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService.ArtifactConditionService;

public class ArtifactConditionService : IArtifactConditionService
{
    private readonly IArtifactConditionRepository _artifactConditionRepository;

    public ArtifactConditionService(IArtifactConditionRepository artifactConditionRepository)
    {
        _artifactConditionRepository = artifactConditionRepository;
    }

    public async Task<List<ArtifactCondition>> GetAllArtifactConditions()
    {
        var artifactConditions = await _artifactConditionRepository.GetAllArtifactConditions();
        return artifactConditions;

    }
}