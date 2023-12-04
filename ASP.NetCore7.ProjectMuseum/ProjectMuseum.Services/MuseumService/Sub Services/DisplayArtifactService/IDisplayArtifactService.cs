using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.DisplayArtifactService;

public interface IDisplayArtifactService
{
    Task<List<Artifact>?> GetAllArtifacts();
    Task<Artifact?> GetArtifactById(string id);
    Task<Artifact?> AddArtifact(Artifact artifact);
    Task<Artifact?> RemoveArtifactById(string id);
}