using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactEraRepository;

public class ArtifactEraRepository : IArtifactEraRepository
{
    private readonly JsonFileDatabase<ArtifactEra> _artifactEraDatabase;

    public ArtifactEraRepository(JsonFileDatabase<ArtifactEra> artifactEraDatabase)
    {
        _artifactEraDatabase = artifactEraDatabase;
    }
    public Task<float> GetEraValueByEra(string era)
    {
        throw new NotImplementedException();
    }
}