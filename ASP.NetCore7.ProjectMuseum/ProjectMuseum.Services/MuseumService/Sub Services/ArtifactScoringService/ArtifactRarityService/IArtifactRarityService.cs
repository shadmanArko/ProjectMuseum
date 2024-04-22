using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService.ArtifactRarityService;

public interface IArtifactRarityService
{
    Task<List<ArtifactRarity>> GetAllArtifactRarity();
}