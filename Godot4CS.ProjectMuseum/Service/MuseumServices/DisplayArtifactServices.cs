using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class DisplayArtifactServices: Node
{
    private DisplayArtifacts _displayArtifactDatabase;
    public override void _EnterTree()
    {
        base._EnterTree();
        var displayArtifactJson = File.ReadAllText(
            "E:/Godot Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Dummy Data Folder/displayArtifact.json");
        _displayArtifactDatabase = SaveLoadService.Load().DisplayArtifacts;
        // GD.Print($"Got display Artifacts {_displayArtifactDatabase[0].Artifacts.Count}");
    }

    public override void _Ready()
    {
        base._Ready();
        
    }

    public List<Artifact> GetAllArtifacts()
    {
        var listOfDisplayArtifact =  _displayArtifactDatabase;
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