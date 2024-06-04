using ProjectMuseum.Models.CoreShop;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ShopService;

public interface IShopService
{
    Task<List<CoreShopDescriptive>?> GetAllShopDescriptive();
    Task<List<CoreShopFunctional>?> GetAllShopFunctional();
}