using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineArtifactRepository;

public interface IMineArtifactRepository
{
    Task<Artifact> AddArtifact(Artifact artifact);
    Task<Artifact> RemoveArtifactById(string id);
    Task<Artifact?> GetArtifactById(string id);
    Task<List<Artifact>?> GetAllArtifacts();
}