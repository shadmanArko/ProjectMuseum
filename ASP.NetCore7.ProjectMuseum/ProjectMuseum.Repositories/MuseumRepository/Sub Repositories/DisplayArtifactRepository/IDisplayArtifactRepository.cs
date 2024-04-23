using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.DisplayArtifactRepository;

public interface IDisplayArtifactRepository
{
    Task<List<Artifact>?> GetAllArtifacts();
    Task<Artifact> AddArtifact(Artifact artifact);
    Task<Artifact> RemoveArtifactById(string id);
    Task<Artifact> GetArtifactById(string id);
}