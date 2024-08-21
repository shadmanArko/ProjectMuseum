using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;
using ProjectMuseum.Models.Artifact_and_Inventory;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class ArtifactStoreServices: Node
{
    private ArtifactStorage _artifactStorageDatabase;
    private MuseumRunningDataContainer _museumRunningDataContainer;
    public override void _Ready()
    {
        base._Ready();
        _artifactStorageDatabase = SaveLoadService.Load().ArtifactStorage;
        _museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
        _museumRunningDataContainer.ArtifactStorage = _artifactStorageDatabase;
    }
    


    public  List<Artifact> GetAllArtifacts()
    {
        var listOfArtifactStorage =  _museumRunningDataContainer.ArtifactStorage;
        var artifactStorage = listOfArtifactStorage;
        var artifacts = artifactStorage?.Artifacts;
        return artifacts;
    }

    public  Artifact AddArtifact(Artifact artifact)
    {
        var listOfArtifactStorage = _artifactStorageDatabase;
        var artifactStorage = listOfArtifactStorage;
        var artifacts = artifactStorage?.Artifacts;
        artifacts?.Add(artifact);
        return artifact;
    }

    public List<Artifact> AddListOfArtifacts(List<Artifact> listOfArtifacts)
    {
        var listOfArtifactStorage = _artifactStorageDatabase;
        var artifactStorage = listOfArtifactStorage;
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
        var artifactStorage = listOfArtifactStorage;
        var artifacts = artifactStorage?.Artifacts;
        var artifactToRemove = artifacts.FirstOrDefault(artifact => artifact.Id == id); //todo If this reference causes any problem please make a new instance of an object
        artifacts.Remove(artifactToRemove);
        return artifactToRemove;
    }
}