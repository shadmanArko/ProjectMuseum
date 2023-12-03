using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactFunctionalRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService;

public class RawArtifactFunctionalService : IRawArtifactFunctionalService
{
    private readonly IRawArtifactFunctionalRepository _rawArtifactFunctionalRepository;

    public RawArtifactFunctionalService(IRawArtifactFunctionalRepository rawArtifactFunctionalRepository)
    {
        _rawArtifactFunctionalRepository = rawArtifactFunctionalRepository;
    }

    public async Task<List<RawArtifactFunctional>?> GetAllRawArtifactFunctional()
    {
        return await _rawArtifactFunctionalRepository.GetAllRawArtifactFunctional();
    }
}