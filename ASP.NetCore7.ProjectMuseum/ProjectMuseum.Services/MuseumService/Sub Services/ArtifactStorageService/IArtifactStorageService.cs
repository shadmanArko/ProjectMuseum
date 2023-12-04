using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactStorageService;

public interface IArtifactStorageService
{
    Task<List<Artifact>?> GetAllArtifactsOfStorage();
    Task<Artifact?> SendArtifactToDisplayById(string id);
    Task<Artifact?> GetArtifactOutOfDisplayById(string id);
    Task<List<Artifact>?> AddListOfArtifacts(List<Artifact> artifacts);
}