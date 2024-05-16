using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.ProductRepository;

public interface IProductRepository
{
    Task<List<Product>?> GetAllProducts();
}