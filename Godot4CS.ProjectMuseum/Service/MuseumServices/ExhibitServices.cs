using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class ExhibitServices: Node
{
    private  List<Exhibit> _exhibitDatabase;
    private  List<ExhibitVariation> _exhibitVariationDatabase;
    private MuseumRunningDataContainer _museumRunningDataContainer;

    public override void _Ready()
    {
        base._Ready();
        
        InitializeData();
    }

    private void InitializeData()
    {
        _exhibitDatabase = SaveLoadService.Load().Exhibits;
        _museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
        _museumRunningDataContainer.Exhibits = _exhibitDatabase;
        var exhibitVariationDatabseJson = Godot.FileAccess.Open("res://Game Data/Starting Data/exhibitVariations.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
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

    public Exhibit AddArtifactToExhibit(string exhibitId, string artifactId, int slot, int gridNumber, string artifactSize)
    {
        var exhibits = _museumRunningDataContainer.Exhibits; ;
        var exhibit = exhibits!.FirstOrDefault(tile => tile.Id == exhibitId);
        // if (exhibit.ArtifactGridSlots2X2s == null)
        // {
        //     Console.WriteLine("2x2 grid is null");
        //     exhibit.ArtifactGridSlots2X2s = new List<GridSlots2X2>(){new GridSlots2X2(){Slot0 = "", Slot1 = "", Slot2 = "", Slot3 = ""}};
        //     
        // }
        if (artifactSize == "Small")
        {
            if (slot == 3 || slot == 1)
            {
                slot = 1;
                exhibit.ArtifactGridSlots2X2s[gridNumber].Slot0 = artifactId;
            }else if (slot == 2 || slot == 4)
            {
                slot = 2;
                exhibit.ArtifactGridSlots2X2s[gridNumber].Slot1 = artifactId;
            }
        }
        else if (artifactSize == "Medium")
        {
            if (slot == 1 || slot == 2 || slot == 3 || slot == 4)
            {
                slot = 1;
                exhibit.ArtifactGridSlots2X2s[gridNumber].Slot0 = artifactId;
            }
        }
        else if (artifactSize == "Tiny")
        {
            if (slot == 1)
            {
                exhibit.ArtifactGridSlots2X2s[gridNumber].Slot0 = artifactId;
            }else if (slot == 2)
            {
                exhibit.ArtifactGridSlots2X2s[gridNumber].Slot1 = artifactId;
            }else if (slot == 3)
            {
                exhibit.ArtifactGridSlots2X2s[gridNumber].Slot2 = artifactId;
            }else if (slot == 4)
            {
                exhibit.ArtifactGridSlots2X2s[gridNumber].Slot3 = artifactId;
            }
        }
        
        exhibit.ArtifactIds.Add(artifactId);
        
        var artifact = _museumRunningDataContainer.ArtifactStorage.Artifacts.FirstOrDefault(artifact => artifact.Id == artifactId);
        _museumRunningDataContainer.ArtifactStorage.Artifacts.Remove(artifact);
        _museumRunningDataContainer.DisplayArtifacts.Artifacts.Add(artifact);
        return exhibit;
    }
    public  Exhibit RemoveArtifactFromExhibit(string exhibitId, string artifactId, int slot, int gridNumber, string artifactSize)
    {
        var exhibits = _museumRunningDataContainer.Exhibits;
        var exhibit = exhibits!.FirstOrDefault(tile => tile.Id == exhibitId);
        foreach (var gridSlots2X2 in exhibit.ArtifactGridSlots2X2s)
        {
            if (gridSlots2X2.Slot0 == artifactId)
            {
                gridSlots2X2.Slot0 = "";
            }else if (gridSlots2X2.Slot1 == artifactId)
            {
                gridSlots2X2.Slot1 = "";
            }else if (gridSlots2X2.Slot2 == artifactId)
            {
                gridSlots2X2.Slot2 = "";
            }else if (gridSlots2X2.Slot3 == artifactId)
            {
                gridSlots2X2.Slot3 = "";
            }
        }
        exhibit.ArtifactIds.Remove(artifactId);
        var artifact = _museumRunningDataContainer.DisplayArtifacts.Artifacts.FirstOrDefault(artifact => artifact.Id == artifactId);
        _museumRunningDataContainer.DisplayArtifacts.Artifacts.Remove(artifact);
        _museumRunningDataContainer.ArtifactStorage.Artifacts.Add(artifact);
        return exhibit;
    }
}