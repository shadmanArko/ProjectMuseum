using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.MineArtifactService;

public interface IMineArtifactService
{
    Task<List<Artifact>?> GetAllArtifactsOfMine();
    Task<Artifact> SendArtifactToInventory(string artifactId);
    Task<List<Artifact>> GenerateNewArtifacts(List<Artifact> listOfArtifacts);
    Task<Artifact?> GetArtifactById(string id);
}