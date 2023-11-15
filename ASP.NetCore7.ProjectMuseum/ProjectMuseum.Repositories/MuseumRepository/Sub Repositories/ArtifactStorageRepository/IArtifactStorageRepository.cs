using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;

public interface IArtifactStorageRepository
{
    Task<List<Artifact>?> GetAllArtifacts();
    Task<Artifact> AddArtifact(Artifact artifact);
    Task<List<Artifact>?> AddListOfArtifacts(List<Artifact> listOfArtifacts);
    Task<Artifact> RemoveArtifactById(string id);
}