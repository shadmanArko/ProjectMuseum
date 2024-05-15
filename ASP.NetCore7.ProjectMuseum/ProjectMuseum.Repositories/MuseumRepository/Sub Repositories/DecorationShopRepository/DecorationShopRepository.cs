using ProjectMuseum.Models;
using ProjectMuseum.Repositories.DecorationRepository;

namespace ProjectMuseum.Repositories.DecorationShopRepository;

public class DecorationShopRepository : IDecorationShopRepository
{
    private readonly JsonFileDatabase<Shop> _decorationShopDatabase;
    private readonly JsonFileDatabase<DecorationShopVariation> _decorationShopVariationDatabase;

    public DecorationShopRepository(JsonFileDatabase<Shop> decorationShopDatabase, JsonFileDatabase<DecorationShopVariation> decorationShopVariationDatabase)
    {
        _decorationShopDatabase = decorationShopDatabase;
        _decorationShopVariationDatabase = decorationShopVariationDatabase;
    }
    public async Task<Shop> Insert(Shop shop)
    {
        var decorationShops = await _decorationShopDatabase.ReadDataAsync();
        decorationShops?.Add(shop);
        if (decorationShops != null) await _decorationShopDatabase.WriteDataAsync(decorationShops);
        return shop;
    }

    public Task<Shop> Update(string id, Shop shop)
    {
        throw new NotImplementedException();
    }

    public Task<Shop?> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Shop>?> GetAll()
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

    public Task<List<Shop>?> GetAllDecorationShops()
    {
        throw new NotImplementedException();
    }

    public Task<Shop?> Delete(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Shop>?> DeleteAllDecorationShops()
    {
        throw new NotImplementedException();
    }

    public Task<Shop?> AddArtifactToDecorationShop(string DecorationShopId, string artifactId, int slot)
    {
        throw new NotImplementedException();
    }

    public Task<Shop?> RemoveArtifactFromDecorationShop(string DecorationShopId, string artifactId, int slot)
    {
        throw new NotImplementedException();
    }
}