namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactEraRepository;

public interface IArtifactEraRepository
{
    Task<float> GetEraValueByEra(string era);
}