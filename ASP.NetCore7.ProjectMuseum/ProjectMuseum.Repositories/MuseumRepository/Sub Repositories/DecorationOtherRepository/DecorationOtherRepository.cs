using ProjectMuseum.Models;
using ProjectMuseum.Repositories.DecorationRepository;

namespace ProjectMuseum.Repositories.DecorationOtherRepository;

public class DecorationOtherRepository : IDecorationOtherRepository
{
    private readonly JsonFileDatabase<DecorationOther> _decorationOtherDatabase;
    private readonly JsonFileDatabase<DecorationOtherVariation> _decorationOtherVariationDatabase;

    //todo problem when uncomment following lines
    public DecorationOtherRepository(JsonFileDatabase<DecorationOther> decorationOtherDatabase, JsonFileDatabase<DecorationOtherVariation> decorationOtherVariationDatabase)
    {
        _decorationOtherDatabase = decorationOtherDatabase;
        _decorationOtherVariationDatabase = decorationOtherVariationDatabase;
    }
    public async Task<DecorationOther> Insert(DecorationOther decorationOther)
    {
        var decorationOthers = await _decorationOtherDatabase.ReadDataAsync();
        decorationOthers?.Add(decorationOther);
        if (decorationOthers != null) await _decorationOtherDatabase.WriteDataAsync(decorationOthers);
        return decorationOther;
    }

    public Task<DecorationOther> Update(string id, DecorationOther DecorationOther)
    {
        throw new NotImplementedException();
    }

    public Task<DecorationOther?> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<DecorationOther>?> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<List<DecorationOtherVariation>?> GetAllDecorationOtherVariations()
    {
        var decorationOtherVariations = await _decorationOtherVariationDatabase.ReadDataAsync();
        return decorationOtherVariations;
    }

    public Task<DecorationOtherVariation?> GetDecorationOtherVariation(string variationName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<DecorationOther>?> GetAllDecorationOthers()
    {
        var decorationOther = await _decorationOtherDatabase.ReadDataAsync();
        return decorationOther;
    }

    public Task<DecorationOther?> Delete(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<DecorationOther>?> DeleteAllDecorationOthers()
    {
        throw new NotImplementedException();
    }

    public Task<DecorationOther?> AddArtifactToDecorationOther(string DecorationOtherId, string artifactId, int slot)
    {
        throw new NotImplementedException();
    }

    public Task<DecorationOther?> RemoveArtifactFromDecorationOther(string DecorationOtherId, string artifactId, int slot)
    {
        throw new NotImplementedException();
    }
}