using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactRarityRepository;

public class ArtifactRarityRepository : IArtifactRarityRepository
{
    private readonly JsonFileDatabase<ArtifactRarity> _artifactRarityDatabase;

    public ArtifactRarityRepository(JsonFileDatabase<ArtifactRarity> artifactRarityDatabase)
    {
        _artifactRarityDatabase = artifactRarityDatabase;
    }

    public Task<float> GetRarityValueByRarity(string rarity)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ArtifactRarity>> GetAllArtifactRarity()
    {
        var artifactRarities = await _artifactRarityDatabase.ReadDataAsync();
        return artifactRarities!;
    }
}