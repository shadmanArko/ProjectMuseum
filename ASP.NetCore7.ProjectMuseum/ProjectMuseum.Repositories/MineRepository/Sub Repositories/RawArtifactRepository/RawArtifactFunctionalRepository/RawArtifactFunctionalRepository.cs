using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactFunctionalRepository;

public class RawArtifactFunctionalRepository : IRawArtifactFunctionalRepository
{
    private readonly JsonFileDatabase<RawArtifactFunctional> _rawArtifactFunctionalDatabase;

    public RawArtifactFunctionalRepository(JsonFileDatabase<RawArtifactFunctional> rawArtifactFunctionalDatabase)
    {
        _rawArtifactFunctionalDatabase = rawArtifactFunctionalDatabase;
    }

    public async Task<List<RawArtifactFunctional>?> GetAllRawArtifactFunctional()
    {
        return await _rawArtifactFunctionalDatabase.ReadDataAsync();
    }
}