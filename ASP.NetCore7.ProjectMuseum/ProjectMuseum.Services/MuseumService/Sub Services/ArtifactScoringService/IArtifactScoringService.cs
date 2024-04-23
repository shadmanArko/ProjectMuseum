using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService;

public interface IArtifactScoringService
{
    Task<List<ArtifactScore>?> RefreshArtifactScore();
}