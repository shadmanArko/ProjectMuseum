using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;

public class ArtifactStorageRepository : IArtifactStorageRepository
{
    private readonly JsonFileDatabase<ArtifactStorage> _artifactStorageDatabase;

    public ArtifactStorageRepository(JsonFileDatabase<ArtifactStorage> artifactStorageDatabase)
    {
        _artifactStorageDatabase = artifactStorageDatabase;
    }


    public async Task<List<Artifact>?> GetAllArtifacts()
    {
        var listOfArtifactStorage = await _artifactStorageDatabase.ReadDataAsync();
        var artifactStorage = listOfArtifactStorage?[0];
        var artifacts = artifactStorage?.Artifacts;
        return artifacts;
    }

    public async Task<Artifact> AddArtifact(Artifact artifact)
    {
        var listOfArtifactStorage = await _artifactStorageDatabase.ReadDataAsync();
        var artifactStorage = listOfArtifactStorage?[0];
        var artifacts = artifactStorage?.Artifacts;
        artifacts?.Add(artifact);
        if (listOfArtifactStorage != null) await _artifactStorageDatabase.WriteDataAsync(listOfArtifactStorage);
        return artifact;
    }

    public async Task<List<Artifact>?> AddListOfArtifacts(List<Artifact> listOfArtifacts)
    {
        var listOfArtifactStorage = await _artifactStorageDatabase.ReadDataAsync();
        var artifactStorage = listOfArtifactStorage?[0];
        var artifacts = artifactStorage?.Artifacts;
        foreach (var artifact in listOfArtifacts)
        {
            artifacts?.Add(artifact);
        }
        if (listOfArtifactStorage != null) await _artifactStorageDatabase.WriteDataAsync(listOfArtifactStorage);
        return artifacts;
    }

    public async Task<Artifact> RemoveArtifactById(string id)
    {
        var listOfArtifactStorage = await _artifactStorageDatabase.ReadDataAsync();
        var artifactStorage = listOfArtifactStorage?[0];
        var artifacts = artifactStorage?.Artifacts;
        var artifactToRemove = artifacts.FirstOrDefault(artifact => artifact.Id == id); //todo If this reference causes any problem please make a new instance of an object
        artifacts.Remove(artifactToRemove);
        if (listOfArtifactStorage != null) await _artifactStorageDatabase.WriteDataAsync(listOfArtifactStorage);
        return artifactToRemove;
    }
}