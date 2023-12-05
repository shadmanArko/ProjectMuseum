using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactDescriptiveRepository;

public class RawArtifactDescriptiveRepository : IRawArtifactDescriptiveRepository
{
    private readonly JsonFileDatabase<RawArtifactDescriptive> _rawArtifactDescriptiveDatabase;

    public RawArtifactDescriptiveRepository(JsonFileDatabase<RawArtifactDescriptive> rawArtifactDescriptiveDatabase)
    {
        _rawArtifactDescriptiveDatabase = rawArtifactDescriptiveDatabase;
    }

    public async Task<List<RawArtifactDescriptive>?> GetAllRawArtifactDescriptive()
    {
        return await _rawArtifactDescriptiveDatabase.ReadDataAsync();
    }
}