using ProjectMuseum.Models.CoreShop;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.ShopRepository;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ShopService;

public class ShopService:IShopService
{
    private IShopRepository _shopRepository;

    public ShopService(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;
    }
    
    public async Task<List<CoreShopDescriptive>?> GetAllShopDescriptive()
    {
        var datas = await _shopRepository.GetAllShopDescriptive();
        return datas;
    }

    public async Task<List<CoreShopFunctional>?> GetAllShopFunctional()
    {
        var datas = await _shopRepository.GetAllShopFunctional();
        return datas;
    }
}