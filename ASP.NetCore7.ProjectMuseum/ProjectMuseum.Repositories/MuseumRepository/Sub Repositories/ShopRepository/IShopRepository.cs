using ProjectMuseum.Models;
using ProjectMuseum.Models.CoreShop;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.ShopRepository;

public interface IShopRepository
{
    Task<List<CoreShopDescriptive>?> GetAllShopDescriptive();
    Task<List<CoreShopFunctional>?> GetAllShopFunctional();
}