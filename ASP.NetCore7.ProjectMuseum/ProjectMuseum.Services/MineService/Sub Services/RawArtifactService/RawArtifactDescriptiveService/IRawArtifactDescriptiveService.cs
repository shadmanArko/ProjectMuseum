using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService.RawArtifactDescriptiveService;

public interface IRawArtifactDescriptiveService
{
    Task<List<RawArtifactDescriptive>?> GetAllRawArtifactDescriptive();
}