using ProjectMuseum.Models.CoreShop;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.ShopRepository;

public class ShopRepository: IShopRepository
{
    private readonly JsonFileDatabase<CoreShopDescriptive> _coreShopDescriptives;
    private readonly JsonFileDatabase<CoreShopFunctional> _coreShopFunctionals;

    public ShopRepository(JsonFileDatabase<CoreShopDescriptive> coreShopDescriptives, JsonFileDatabase<CoreShopFunctional> coreShopFunctionals)
    {
        _coreShopDescriptives = coreShopDescriptives;
        _coreShopFunctionals = coreShopFunctionals;
    }

    public async Task<List<CoreShopDescriptive>?> GetAllShopDescriptive()
    {
        var datas = await _coreShopDescriptives.ReadDataAsync();
        return datas;
    }

    public async Task<List<CoreShopFunctional>?> GetAllShopFunctional()
    {
        var datas = await _coreShopFunctionals.ReadDataAsync();
        return datas;
    }
}