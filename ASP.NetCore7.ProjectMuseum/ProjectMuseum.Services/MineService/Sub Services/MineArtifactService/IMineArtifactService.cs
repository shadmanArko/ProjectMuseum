using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services;

public interface IMineArtifactService
{
    Task<List<Artifact>> GetAllArtifactsOfMine();
    Task<Artifact> SendArtifactToInventory();
}