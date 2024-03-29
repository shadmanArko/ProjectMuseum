using ProjectMuseum.Models;

namespace ProjectMuseum.Services.DecorationShopServices;

public interface IDecorationShopService
{
    Task<List<DecorationShopVariation>?> GetAllDecorationShopVariations();
    Task<DecorationShopVariation?> GetDecorationShopVariation(string variationName);
    Task<List<DecorationShop>?> DeleteAllDecorationShops();
    Task<List<DecorationShop>?> GetAllDecorationShops();
    
}