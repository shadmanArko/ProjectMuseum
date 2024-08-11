using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class ExhibitServices: Node
{
    private  List<Exhibit> _exhibitDatabase;
    private  List<ExhibitVariation> _exhibitVariationDatabase;
   

    public override void _Ready()
    {
        base._Ready();
        InitializeData();
    }

    private void InitializeData()
    {
        var exhibitDatabseJson = File.ReadAllText(
            "E:/Godot Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Dummy Data Folder/exhibit.json");
        _exhibitDatabase = JsonSerializer.Deserialize<List<Exhibit>>(exhibitDatabseJson);

        var exhibitVariationDatabseJson = File.ReadAllText(
            "E:/Godot Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Dummy Data Folder/exhibitVariations.json");
        _exhibitVariationDatabase = JsonSerializer.Deserialize<List<ExhibitVariation>>(exhibitVariationDatabseJson);
    }

    public Exhibit Insert(Exhibit exhibit)
    {
        var exhibits = _exhibitDatabase;
        exhibits?.Add(exhibit);
        if (exhibits != null) _exhibitDatabase = exhibits;
        return exhibit;
    }

    public Exhibit Update(string id, Exhibit exhibit)
    {
        var exhibits = _exhibitDatabase;
        var foundExhibit = exhibits.FirstOrDefault(exhibit1 => exhibit1.Id == exhibit.Id);
        if (foundExhibit != null)
        {
            exhibits.Remove(foundExhibit);
        }
        exhibits.Add(exhibit);
        if (exhibits != null) _exhibitDatabase = exhibits;
        return exhibit;
    }

    public Exhibit GetById(string id)
    {
        var exhibits = _exhibitDatabase;
        var exhibit = exhibits!.FirstOrDefault(tile => tile.Id == id);
        return exhibit;
    }

    public List<Exhibit> GetAll()
    {
        var exhibits = _exhibitDatabase;
        return exhibits;
    }

    public List<ExhibitVariation> GetAllExhibitVariations()
    {
        var exhibitVariations = _exhibitVariationDatabase;
        return exhibitVariations;
    }

    public ExhibitVariation GetExhibitVariation(string variationName)
    {
        var exhibitVariations = _exhibitVariationDatabase;
        var exhibitVariation = exhibitVariations!.FirstOrDefault(variation => variation.VariationName == variationName);
        return exhibitVariation;
    }

    public List<Exhibit> GetAllExhibits()
    {
        InitializeData();
        var exhibits = _exhibitDatabase;
        return exhibits;
    }

    public List<Exhibit> Delete(string id)
    {
        var exhibits = _exhibitDatabase;
        var exhibit = exhibits!.FirstOrDefault(tile => tile.Id == id);
        if (exhibit != null) exhibits?.Remove(exhibit);
        if (exhibits != null) _exhibitDatabase = exhibits;
        return _exhibitDatabase;
    }

    public List<Exhibit> DeleteAllExhibits()
    {
        var exhibits = new List<Exhibit>();
        _exhibitDatabase = exhibits;
        return exhibits;
    }

    public Exhibit AddArtifactToExhibit(string exhibitId, string artifactId, int slot)
    {
        var exhibits = _exhibitDatabase;
        var exhibit = exhibits!.FirstOrDefault(tile => tile.Id == exhibitId);
        if (slot == 1)
        {
            exhibit.ExhibitArtifactSlot1 = artifactId;
        }else if (slot == 2)
        {
            exhibit.ExhibitArtifactSlot2 = artifactId;
        }else if (slot == 3)
        {
            exhibit.ExhibitArtifactSlot3 = artifactId;
        }else if (slot == 4)
        {
            exhibit.ExhibitArtifactSlot4 = artifactId;
        }
        exhibit.ArtifactIds.Add(artifactId);
        if (exhibits != null) _exhibitDatabase = exhibits;
        return exhibit;
    }
    public Exhibit RemoveArtifactFromExhibit(string exhibitId, string artifactId, int slot)
    {
        var exhibits = _exhibitDatabase;
        var exhibit = exhibits!.FirstOrDefault(tile => tile.Id == exhibitId);
        if (slot == 1)
        {
            exhibit.ExhibitArtifactSlot1 = "";
        }else if (slot == 2)
        {
            exhibit.ExhibitArtifactSlot2 = "";
        }
        else if (slot == 3)
        {
            exhibit.ExhibitArtifactSlot3 = "";
        }
        else if (slot == 4)
        {
            exhibit.ExhibitArtifactSlot4 = "";
        }
        exhibit.ArtifactIds.Remove(artifactId);

        if (exhibits != null) _exhibitDatabase = exhibits;
        return exhibit;
    }
}