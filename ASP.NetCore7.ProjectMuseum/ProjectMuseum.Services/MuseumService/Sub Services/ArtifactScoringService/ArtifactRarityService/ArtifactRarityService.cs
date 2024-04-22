using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactRarityRepository;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService.ArtifactRarityService;

public class ArtifactRarityService : IArtifactRarityService
{
    private readonly IArtifactRarityRepository _artifactRarityRepository;

    public ArtifactRarityService(IArtifactRarityRepository artifactRarityRepository)
    {
        _artifactRarityRepository = artifactRarityRepository;
    }

    public async Task<List<ArtifactRarity>> GetAllArtifactRarity()
    {
        var artifactRarities = await _artifactRarityRepository.GetAllArtifactRarity();
        return artifactRarities;
    }
}