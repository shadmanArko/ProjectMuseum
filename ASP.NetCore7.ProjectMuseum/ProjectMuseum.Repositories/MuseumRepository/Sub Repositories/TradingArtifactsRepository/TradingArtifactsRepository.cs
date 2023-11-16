using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.TradingArtifactRepository;

public class TradingArtifactsRepository : ITradingArtifactsRepository
{
    private readonly JsonFileDatabase<TradingArtifacts> _tradingArtifactsDatabase;

    public TradingArtifactsRepository(JsonFileDatabase<TradingArtifacts> tradingArtifactsDatabase)
    {
        _tradingArtifactsDatabase = tradingArtifactsDatabase;
    }

    public async Task<Artifact> AddArtifact(Artifact artifact)
    {
        var listOfTradingArtifacts = await _tradingArtifactsDatabase.ReadDataAsync();
        var tradingArtifacts = listOfTradingArtifacts?[0];
        var artifacts = tradingArtifacts?.Artifacts;
        artifacts?.Add(artifact);
        if (listOfTradingArtifacts != null) await _tradingArtifactsDatabase.WriteDataAsync(listOfTradingArtifacts);
        return artifact;
    }

    public async Task<List<Artifact>?> GetAllArtifacts()
    {
        var listOfTradingArtifacts = await _tradingArtifactsDatabase.ReadDataAsync();
        var tradingArtifacts = listOfTradingArtifacts?[0];
        var artifacts = tradingArtifacts?.Artifacts;
        return artifacts;
    }
}