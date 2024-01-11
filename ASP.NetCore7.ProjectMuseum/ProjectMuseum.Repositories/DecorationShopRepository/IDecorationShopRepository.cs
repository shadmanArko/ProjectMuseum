using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.DecorationRepository;

public interface IDecorationShopRepository
{
    Task<DecorationShop> Insert(DecorationShop DecorationShop);
    Task<DecorationShop> Update(string id, DecorationShop DecorationShop);
    Task<DecorationShop?> GetById(string id);
    Task<List<DecorationShop>?> GetAll();
    Task<List<DecorationShopVariation>?> GetAllDecorationShopVariations();
    Task<DecorationShopVariation?> GetDecorationShopVariation(string variationName);
    Task<List<DecorationShop>?> GetAllDecorationShops();
    Task<DecorationShop?> Delete(string id);
    Task<List<DecorationShop>?> DeleteAllDecorationShops();
    Task<DecorationShop?> AddArtifactToDecorationShop(string DecorationShopId, string artifactId, int slot);
    Task<DecorationShop?> RemoveArtifactFromDecorationShop(string DecorationShopId, string artifactId, int slot);
}