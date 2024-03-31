using ProjectMuseum.Models;
using ProjectMuseum.Repositories.DecorationRepository;

namespace ProjectMuseum.Repositories.DecorationShopRepository;

public class DecorationShopRepository : IDecorationShopRepository
{
    private readonly JsonFileDatabase<DecorationShop> _decorationShopDatabase;
    private readonly JsonFileDatabase<DecorationShopVariation> _decorationShopVariationDatabase;

    public DecorationShopRepository(JsonFileDatabase<DecorationShop> decorationShopDatabase, JsonFileDatabase<DecorationShopVariation> decorationShopVariationDatabase)
    {
        _decorationShopDatabase = decorationShopDatabase;
        _decorationShopVariationDatabase = decorationShopVariationDatabase;
    }
    public async Task<DecorationShop> Insert(DecorationShop decorationShop)
    {
        var decorationShops = await _decorationShopDatabase.ReadDataAsync();
        decorationShops?.Add(decorationShop);
        if (decorationShops != null) await _decorationShopDatabase.WriteDataAsync(decorationShops);
        return decorationShop;
    }

    public Task<DecorationShop> Update(string id, DecorationShop DecorationShop)
    {
        throw new NotImplementedException();
    }

    public Task<DecorationShop?> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<DecorationShop>?> GetAll()
    {
        var decorationShop = await _decorationShopDatabase.ReadDataAsync();
        return decorationShop;
    }

    public async Task<List<DecorationShopVariation>?> GetAllDecorationShopVariations()
    {
        var decorationShopVariations = await _decorationShopVariationDatabase.ReadDataAsync();
        return decorationShopVariations;
    }

    public Task<DecorationShopVariation?> GetDecorationShopVariation(string variationName)
    {
        throw new NotImplementedException();
    }

    public Task<List<DecorationShop>?> GetAllDecorationShops()
    {
        throw new NotImplementedException();
    }

    public Task<DecorationShop?> Delete(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<DecorationShop>?> DeleteAllDecorationShops()
    {
        throw new NotImplementedException();
    }

    public Task<DecorationShop?> AddArtifactToDecorationShop(string DecorationShopId, string artifactId, int slot)
    {
        throw new NotImplementedException();
    }

    public Task<DecorationShop?> RemoveArtifactFromDecorationShop(string DecorationShopId, string artifactId, int slot)
    {
        throw new NotImplementedException();
    }
}