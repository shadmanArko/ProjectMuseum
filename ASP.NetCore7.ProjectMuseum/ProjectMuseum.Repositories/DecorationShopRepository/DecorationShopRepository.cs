using ProjectMuseum.Models;
using ProjectMuseum.Repositories.DecorationRepository;

namespace ProjectMuseum.Repositories.DecorationShopRepository;

public class DecorationShopRepository : IDecorationShopRepository
{
    public Task<DecorationShop> Insert(DecorationShop DecorationShop)
    {
        throw new NotImplementedException();
    }

    public Task<DecorationShop> Update(string id, DecorationShop DecorationShop)
    {
        throw new NotImplementedException();
    }

    public Task<DecorationShop?> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<DecorationShop>?> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<DecorationShopVariation>?> GetAllDecorationShopVariations()
    {
        throw new NotImplementedException();
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