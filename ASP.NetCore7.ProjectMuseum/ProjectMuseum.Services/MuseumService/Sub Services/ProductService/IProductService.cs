using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ProductService;

public interface IProductService
{
    Task<List<Product>?> GetAllProducts();
}