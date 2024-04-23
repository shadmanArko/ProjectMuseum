using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.SanitationRepository;

public class SanitationRepository: ISanitationRepository
{
    private readonly JsonFileDatabase<Sanitation> _sanitationDatabase;
    private readonly JsonFileDatabase<SanitationVariation> _sanitationVariationDatabase;

    public SanitationRepository(JsonFileDatabase<Sanitation> sanitationDatabase, JsonFileDatabase<SanitationVariation> sanitationVariationDatabase)
    {
        _sanitationDatabase = sanitationDatabase;
        _sanitationVariationDatabase = sanitationVariationDatabase;
    }
    public async Task<Sanitation> Insert(Sanitation sanitation)
    {
        var sanitations = await _sanitationDatabase.ReadDataAsync();
        sanitations?.Add(sanitation);
        if (sanitations != null) await _sanitationDatabase.WriteDataAsync(sanitations);
        return sanitation;
    }

    public async Task<Sanitation> Update(string id, Sanitation sanitation)
    {
        var sanitations = await _sanitationDatabase.ReadDataAsync();
        var sanitationToUpdate = sanitation;
        sanitationToUpdate.Id = id;
        if (sanitations != null) await _sanitationDatabase.WriteDataAsync(sanitations);
        return sanitation;
    }

    public async Task<Sanitation?> GetById(string id)
    {
        var sanitations = await _sanitationDatabase.ReadDataAsync();
        var sanitation = sanitations!.FirstOrDefault(tile => tile.Id == id);
        return sanitation;
    }

    public async Task<List<Sanitation>?> GetAll()
    {
        var sanitations = await _sanitationDatabase.ReadDataAsync();
        return sanitations;
    }

    public async Task<List<SanitationVariation>?> GetAllSanitationVariations()
    {
        var sanitationVariations = await _sanitationVariationDatabase.ReadDataAsync();
        return sanitationVariations;
    }

    public async Task<SanitationVariation?> GetSanitationVariation(string variationName)
    {
        var sanitationVariations = await _sanitationVariationDatabase.ReadDataAsync();
        var sanitationVariation = sanitationVariations!.FirstOrDefault(variation => variation.SanitationId == variationName);
        return sanitationVariation;
    }

    public async Task<List<Sanitation>?> GetAllSanitation()
    {
        var sanitations = await _sanitationDatabase.ReadDataAsync();
        return sanitations;
    }

    public async Task<Sanitation?> Delete(string id)
    {
        var sanitations = await _sanitationDatabase.ReadDataAsync();
        var sanitation = sanitations!.FirstOrDefault(tile => tile.Id == id);
        if (sanitation != null) sanitations?.Remove(sanitation);
        if (sanitations != null) await _sanitationDatabase.WriteDataAsync(sanitations);
        return sanitation;
    }

    public async Task<List<Sanitation>?> DeleteAllSanitation()
    {
        var sanitations = new List<Sanitation>();
        await _sanitationDatabase.WriteDataAsync(sanitations);
        return sanitations;
    }
}