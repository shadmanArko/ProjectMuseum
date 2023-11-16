using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.TradingArtifactsService;

public interface ITradingArtifactsService
{
    Task<List<Artifact>?> GetAllArtifacts();
    Task<Artifact> AddArtifact(Artifact artifact);
}