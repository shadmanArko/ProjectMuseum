using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.TradingArtifactRepository;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.TradingArtifactsService;

public class TradingArtifactsService : ITradingArtifactsService
{
    private readonly ITradingArtifactsRepository _tradingArtifactsRepository;

    public TradingArtifactsService(ITradingArtifactsRepository tradingArtifactsRepository)
    {
        _tradingArtifactsRepository = tradingArtifactsRepository;
    }

    public async Task<List<Artifact>?> GetAllArtifacts()
    {
        var artifacts = await _tradingArtifactsRepository.GetAllArtifacts();
        return artifacts;
    }

    public async Task<Artifact> AddArtifact(Artifact artifact)
    {
        return await _tradingArtifactsRepository.AddArtifact(artifact);
    }
}