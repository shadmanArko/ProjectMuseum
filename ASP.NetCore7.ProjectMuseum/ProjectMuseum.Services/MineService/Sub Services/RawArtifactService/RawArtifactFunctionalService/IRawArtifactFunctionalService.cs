using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService;

public interface IRawArtifactFunctionalService
{
    Task<List<RawArtifactFunctional>?> GetAllRawArtifactFunctional();
}