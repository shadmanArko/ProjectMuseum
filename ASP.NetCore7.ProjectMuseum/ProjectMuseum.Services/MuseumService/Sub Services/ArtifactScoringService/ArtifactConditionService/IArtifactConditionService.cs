using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService.ArtifactConditionService;

public interface IArtifactConditionService
{
    Task<List<ArtifactCondition>> GetAllArtifactConditions();
}