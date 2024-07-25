using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.ExhibitRepository;

public class ExhibitRepository : IExhibitRepository
{
    private readonly JsonFileDatabase<Exhibit> _exhibitDatabase;
    private readonly JsonFileDatabase<ExhibitVariation> _exhibitVariationDatabase;

    public ExhibitRepository(JsonFileDatabase<Exhibit> exhibitDatabase, JsonFileDatabase<ExhibitVariation> exhibitVariationDatabase)
    {
        _exhibitDatabase = exhibitDatabase;
        _exhibitVariationDatabase = exhibitVariationDatabase;
    }
 
    public async Task<Exhibit> Insert(Exhibit exhibit)
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        exhibits?.Add(exhibit);
        if (exhibits != null) await _exhibitDatabase.WriteDataAsync(exhibits);
        return exhibit;
    }

    public async Task<Exhibit> Update(string id, Exhibit exhibit)
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        var foundExhibit = exhibits.FirstOrDefault(exhibit1 => exhibit1.Id == exhibit.Id);
        if (foundExhibit != null)
        {
            exhibits.Remove(foundExhibit);
        }
        exhibits.Add(exhibit);
        if (exhibits != null) await _exhibitDatabase.WriteDataAsync(exhibits);
        return exhibit;
    }

    public async Task<Exhibit?> GetById(string id)
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        var exhibit = exhibits!.FirstOrDefault(tile => tile.Id == id);
        return exhibit;
    }

    public async Task<List<Exhibit>?> GetAll()
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        return exhibits;
    }

    public async Task<List<ExhibitVariation>?> GetAllExhibitVariations()
    {
        var exhibitVariations = await _exhibitVariationDatabase.ReadDataAsync();
        return exhibitVariations;
    }

    public async Task<ExhibitVariation?> GetExhibitVariation(string variationName)
    {
        var exhibitVariations = await _exhibitVariationDatabase.ReadDataAsync();
        var exhibitVariation = exhibitVariations!.FirstOrDefault(variation => variation.VariationName == variationName);
        return exhibitVariation;
    }

    public async Task<List<Exhibit>?> GetAllExhibits()
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        return exhibits;
    }

    public async Task<List<Exhibit>?> Delete(string id)
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
        var exhibit = exhibits!.FirstOrDefault(tile => tile.Id == id);
        if (exhibit != null) exhibits?.Remove(exhibit);
        if (exhibits != null) await _exhibitDatabase.WriteDataAsync(exhibits);
        return await _exhibitDatabase.ReadDataAsync();
    }

    public async Task<List<Exhibit>?> DeleteAllExhibits()
    {
        var exhibits = new List<Exhibit>();
        await _exhibitDatabase.WriteDataAsync(exhibits);
        return exhibits;
    }

    public async Task<Exhibit?> AddArtifactToExhibit(string exhibitId, string artifactId, int slot, int gridNumber, string artifactSize)
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
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
        if (exhibits != null) await _exhibitDatabase.WriteDataAsync(exhibits);
        return exhibit;
    }
    public async Task<Exhibit?> RemoveArtifactFromExhibit(string exhibitId, string artifactId, int slot, int gridNumber, string artifactSize)
    {
        var exhibits = await _exhibitDatabase.ReadDataAsync();
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

        if (exhibits != null) await _exhibitDatabase.WriteDataAsync(exhibits);
        return exhibit;
    }
}