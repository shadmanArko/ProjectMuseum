using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactRarityRepository;

public class ArtifactRarityRepository : IArtifactRarityRepository
{
    private readonly JsonFileDatabase<ArtifactRarity> _artifactRarityDatabase;

    public ArtifactRarityRepository(JsonFileDatabase<ArtifactRarity> artifactRarityDatabase)
    {
        _artifactRarityDatabase = artifactRarityDatabase;
    }

    public async Task<float> GetRarityValueByRarity(string rarity)
    {
        var artifactRarities = await _artifactRarityDatabase.ReadDataAsync();
        var artifactRarity = artifactRarities.FirstOrDefault(rarity1 => rarity1.Rarity == rarity);
        return artifactRarity.ScoreMultiplier;
    }

    public async Task<List<ArtifactRarity>> GetAllArtifactRarity()
    {
        var artifactRarities = await _artifactRarityDatabase.ReadDataAsync();
        return artifactRarities!;
    }
}