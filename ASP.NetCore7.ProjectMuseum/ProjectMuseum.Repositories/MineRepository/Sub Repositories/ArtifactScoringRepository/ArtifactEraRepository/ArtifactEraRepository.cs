using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactEraRepository;

public class ArtifactEraRepository : IArtifactEraRepository
{
    private readonly JsonFileDatabase<ArtifactEra> _artifactEraDatabase;

    public ArtifactEraRepository(JsonFileDatabase<ArtifactEra> artifactEraDatabase)
    {
        _artifactEraDatabase = artifactEraDatabase;
    }
    public async Task<float> GetEraValueByEra(string era)
    {
        var artifactEras = await _artifactEraDatabase.ReadDataAsync();
        var artifactEra = artifactEras.FirstOrDefault(era1 => era1.Era == era);
        return artifactEra.ScoreMultiplier;
    }
}