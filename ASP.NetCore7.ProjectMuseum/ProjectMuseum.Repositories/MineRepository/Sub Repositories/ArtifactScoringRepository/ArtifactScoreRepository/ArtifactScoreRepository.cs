using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactScoreRepository;

public class ArtifactScoreRepository : IArtifactScoreRepository
{
    private readonly JsonFileDatabase<ArtifactScore> _artifactScoreDatabase;

    public ArtifactScoreRepository(JsonFileDatabase<ArtifactScore> artifactScoreDatabase)
    {
        _artifactScoreDatabase = artifactScoreDatabase;
    }

    public async Task<List<ArtifactScore>?> GetAllArtifactScore()
    {
        var artifactScores = await _artifactScoreDatabase.ReadDataAsync();
        return artifactScores;
    }

    public Task<List<ArtifactScore>?> UpdateArtifactScore(List<ArtifactScore> artifactScores)
    {
        throw new NotImplementedException();
    }
}