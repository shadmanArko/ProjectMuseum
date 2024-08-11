using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class ArtifactStoreServices: Node
{
    private List<ArtifactStorage> _artifactStorageDatabase;

    public override void _Ready()
    {
        base._Ready();
        var artifactStorageJson = File.ReadAllText(
            "E:/Godot Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Dummy Data Folder/artifactStorage.json");
        _artifactStorageDatabase = JsonSerializer.Deserialize<List<ArtifactStorage>>(artifactStorageJson);
    }
    


    public  List<Artifact> GetAllArtifacts()
    {
        var listOfArtifactStorage =  _artifactStorageDatabase;
        var artifactStorage = listOfArtifactStorage?[0];
        var artifacts = artifactStorage?.Artifacts;
        return artifacts;
    }

    public  Artifact AddArtifact(Artifact artifact)
    {
        var listOfArtifactStorage = _artifactStorageDatabase;
        var artifactStorage = listOfArtifactStorage?[0];
        var artifacts = artifactStorage?.Artifacts;
        artifacts?.Add(artifact);
        return artifact;
    }

    public List<Artifact> AddListOfArtifacts(List<Artifact> listOfArtifacts)
    {
        var listOfArtifactStorage = _artifactStorageDatabase;
        var artifactStorage = listOfArtifactStorage?[0];
        var artifacts = artifactStorage?.Artifacts;
        foreach (var artifact in listOfArtifacts)
        {
            artifacts?.Add(artifact);
        }
        return artifacts;
    }

    public Artifact RemoveArtifactById(string id)
    {
        var listOfArtifactStorage = _artifactStorageDatabase;
        var artifactStorage = listOfArtifactStorage?[0];
        var artifacts = artifactStorage?.Artifacts;
        var artifactToRemove = artifacts.FirstOrDefault(artifact => artifact.Id == id); //todo If this reference causes any problem please make a new instance of an object
        artifacts.Remove(artifactToRemove);
        return artifactToRemove;
    }
}