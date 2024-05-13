using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.DecorationRepository;

public interface IDecorationShopRepository
{
    Task<Shop> Insert(Shop shop);
    Task<Shop> Update(string id, Shop shop);
    Task<Shop?> GetById(string id);
    Task<List<Shop>?> GetAll();
    Task<List<DecorationShopVariation>?> GetAllDecorationShopVariations();
    Task<DecorationShopVariation?> GetDecorationShopVariation(string variationName);
    Task<List<Shop>?> GetAllDecorationShops();
    Task<Shop?> Delete(string id);
    Task<List<Shop>?> DeleteAllDecorationShops();
    Task<Shop?> AddArtifactToDecorationShop(string DecorationShopId, string artifactId, int slot);
    Task<Shop?> RemoveArtifactFromDecorationShop(string DecorationShopId, string artifactId, int slot);
}