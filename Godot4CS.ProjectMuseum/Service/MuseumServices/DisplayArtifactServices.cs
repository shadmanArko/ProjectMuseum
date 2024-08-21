using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class DisplayArtifactServices: Node
{
    private DisplayArtifacts _displayArtifactDatabase;
    private MuseumRunningDataContainer _museumRunningDataContainer;
    public override void _EnterTree()
    {
        base._EnterTree();
        _museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
        _displayArtifactDatabase = SaveLoadService.Load().DisplayArtifacts;
        _museumRunningDataContainer.DisplayArtifacts = _displayArtifactDatabase;
        // GD.Print($"Got display Artifacts {_displayArtifactDatabase[0].Artifacts.Count}");
    }

    public override void _Ready()
    {
        base._Ready();
        
    }

    public List<Artifact> GetAllArtifacts()
    {
        var listOfDisplayArtifact = _museumRunningDataContainer.DisplayArtifacts;
        var displayArtifacts = listOfDisplayArtifact;
        var artifacts = displayArtifacts?.Artifacts;
        GD.Print($"got display artifacts {artifacts.Count}");
        return artifacts;
    }

    public Artifact AddArtifact(Artifact artifact)
    {
        var listOfDisplayArtifact =  _displayArtifactDatabase;
        var displayArtifacts = listOfDisplayArtifact;
        var artifacts = displayArtifacts?.Artifacts;
        artifacts?.Add(artifact);
        return artifact;
    }

    public Artifact RemoveArtifactById(string id)
    {
        var displayArtifactsList =  _displayArtifactDatabase;
        var displayArtifacts = displayArtifactsList;
        var artifacts = displayArtifacts?.Artifacts;
        var artifactToRemove = artifacts.FirstOrDefault(artifact => artifact.Id == id); //todo If this reference causes any problem please make a new instance of an object
        artifacts.Remove(artifactToRemove);
        return artifactToRemove;    
    }

    public Artifact GetArtifactById(string id)
    {
        var listOfDisplayArtifact = _displayArtifactDatabase;
        var displayArtifacts = listOfDisplayArtifact;
        
        var artifacts = displayArtifacts?.Artifacts;
        var artifactToGet = artifacts.FirstOrDefault(artifact => artifact.Id == id);
        return artifactToGet;
    }
}