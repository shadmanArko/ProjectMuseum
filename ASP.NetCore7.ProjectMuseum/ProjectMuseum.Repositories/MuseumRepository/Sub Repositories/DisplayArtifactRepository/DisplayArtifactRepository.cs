using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.DisplayArtifactRepository;

public class DisplayArtifactRepository : IDisplayArtifactRepository
{
    private readonly JsonFileDatabase<DisplayArtifacts> _displayArtifactDatabase;

    public DisplayArtifactRepository(JsonFileDatabase<DisplayArtifacts> displayArtifactDatabase)
    {
        _displayArtifactDatabase = displayArtifactDatabase;
    }

    public async Task<List<Artifact>?> GetAllArtifacts()
    {
        var listOfDisplayArtifact = await _displayArtifactDatabase.ReadDataAsync();
        var displayArtifacts = listOfDisplayArtifact?[0];
        var artifacts = displayArtifacts?.Artifacts;
        return artifacts;
    }

    public async Task<Artifact> AddArtifact(Artifact artifact)
    {
        var listOfDisplayArtifact = await _displayArtifactDatabase.ReadDataAsync();
        var displayArtifacts = listOfDisplayArtifact?[0];
        var artifacts = displayArtifacts?.Artifacts;
        artifacts?.Add(artifact);
        if (listOfDisplayArtifact != null) await _displayArtifactDatabase.WriteDataAsync(listOfDisplayArtifact);
        return artifact;
    }

    public async Task<Artifact> RemoveArtifactById(string id)
    {
        var displayArtifactsList = await _displayArtifactDatabase.ReadDataAsync();
        var displayArtifacts = displayArtifactsList?[0];
        var artifacts = displayArtifacts?.Artifacts;
        var artifactToRemove = artifacts.FirstOrDefault(artifact => artifact.Id == id); //todo If this reference causes any problem please make a new instance of an object
        artifacts.Remove(artifactToRemove);
        if (displayArtifactsList != null) await _displayArtifactDatabase.WriteDataAsync(displayArtifactsList);
        return artifactToRemove;    
    }

    public async Task<Artifact> GetArtifactById(string id)
    {
        var listOfDisplayArtifact = await _displayArtifactDatabase.ReadDataAsync();
        var displayArtifacts = listOfDisplayArtifact?[0];
        
        var artifacts = displayArtifacts?.Artifacts;
        var artifactToGet = artifacts.FirstOrDefault(artifact => artifact.Id == id);
        return artifactToGet;
    }
}