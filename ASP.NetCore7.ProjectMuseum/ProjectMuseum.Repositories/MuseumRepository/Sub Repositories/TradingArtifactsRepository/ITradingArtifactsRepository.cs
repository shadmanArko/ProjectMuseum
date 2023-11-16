using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.TradingArtifactRepository;

public interface ITradingArtifactsRepository
{
    Task<Artifact> AddArtifact(Artifact artifact);
    Task<List<Artifact>?> GetAllArtifacts();
}