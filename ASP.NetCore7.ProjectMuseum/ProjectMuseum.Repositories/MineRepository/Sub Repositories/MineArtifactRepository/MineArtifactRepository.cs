using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineArtifactRepository;

public class MineArtifactRepository : IMineArtifactRepository
{
    private readonly JsonFileDatabase<MineArtifacts> _mineArtifactDatabase;

    public MineArtifactRepository(JsonFileDatabase<MineArtifacts> mineArtifactDatabase)
    {
        _mineArtifactDatabase = mineArtifactDatabase;
    }
    public async Task<Artifact> AddArtifact(Artifact artifact)
    {
        var listOfMineArtifacts = await _mineArtifactDatabase.ReadDataAsync();
        var mineArtifacts = listOfMineArtifacts?[0];
        var artifacts = mineArtifacts?.Artifacts;
        artifacts?.Add(artifact);
        if (listOfMineArtifacts != null) await _mineArtifactDatabase.WriteDataAsync(listOfMineArtifacts);
        return artifact;
    }

    public async Task<Artifact> RemoveArtifactById(string id)
    {
        var listOfMineArtifacts = await _mineArtifactDatabase.ReadDataAsync();
        var mineArtifacts = listOfMineArtifacts?[0];
        var artifacts = mineArtifacts?.Artifacts;
        var artifactToRemove = artifacts.FirstOrDefault(artifact => artifact.Id == id); //todo If this reference causes any problem please make a new instance of an object
        artifacts.Remove(artifactToRemove);
        if (listOfMineArtifacts != null) await _mineArtifactDatabase.WriteDataAsync(listOfMineArtifacts);
        return artifactToRemove;
    }

    public async Task<Artifact?> GetArtifactById(string id)
    {
        var listOfMineArtifacts = await _mineArtifactDatabase.ReadDataAsync();
        var mineArtifacts = listOfMineArtifacts?[0];
        var artifacts = mineArtifacts?.Artifacts;
        var artifact = artifacts.FirstOrDefault(artifact1 => artifact1.Id == id);
        return artifact;
    }

    public async Task<List<Artifact>?> GetAllArtifacts()
    {
        var listOfMineArtifacts = await _mineArtifactDatabase.ReadDataAsync();
        var mineArtifacts = listOfMineArtifacts?[0];
        var artifacts = mineArtifacts?.Artifacts;
        return artifacts;
    }
}