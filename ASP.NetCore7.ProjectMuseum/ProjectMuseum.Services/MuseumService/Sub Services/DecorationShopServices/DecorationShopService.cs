using ProjectMuseum.Models;
using ProjectMuseum.Repositories.DecorationRepository;

namespace ProjectMuseum.Services.DecorationShopServices;

public class DecorationShopService : IDecorationShopService
{
    private IDecorationShopRepository _decorationShopRepository;

    public DecorationShopService(IDecorationShopRepository decorationShopRepository)
    {
        _decorationShopRepository = decorationShopRepository;
    }


    public async Task<List<DecorationShopVariation>?> GetAllDecorationShopVariations()
    {
        return await _decorationShopRepository.GetAllDecorationShopVariations();
    }

    public async Task<DecorationShopVariation?> GetDecorationShopVariation(string variationName)
    {
        return await _decorationShopRepository.GetDecorationShopVariation(variationName);

    }

    public  async Task<List<Shop>?> DeleteAllDecorationShops()
    {
        return await _decorationShopRepository.DeleteAllDecorationShops();

    }

    public async Task<List<Shop>?> GetAllDecorationShops()
    {
        return await _decorationShopRepository.GetAll();

    }
}