using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactDescriptiveRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService.RawArtifactDescriptiveService;

public class RawArtifactDescriptiveService : IRawArtifactDescriptiveService
{
    private readonly IRawArtifactDescriptiveRepository _rawArtifactDescriptiveRepository;

    public RawArtifactDescriptiveService(IRawArtifactDescriptiveRepository rawArtifactDescriptiveRepository)
    {
        _rawArtifactDescriptiveRepository = rawArtifactDescriptiveRepository;
    }

    public async Task<List<RawArtifactDescriptive>?> GetAllRawArtifactDescriptive()
    {
        return await _rawArtifactDescriptiveRepository.GetAllRawArtifactDescriptive();
    }
}