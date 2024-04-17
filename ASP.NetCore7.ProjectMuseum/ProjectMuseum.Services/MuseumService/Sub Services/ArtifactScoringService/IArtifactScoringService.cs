using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService;

public interface IArtifactScoringService
{
    Task<float> GetArtifactScoreWhichIsNotInZone(Artifact artifact);
}