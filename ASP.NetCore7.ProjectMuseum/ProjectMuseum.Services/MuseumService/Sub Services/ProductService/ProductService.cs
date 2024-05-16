using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.ProductRepository;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ProductService;

public class ProductService:IProductService
{
    private IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<List<Product>?> GetAllProducts()
    {
        var datas = await _productRepository.GetAllProducts();
        return datas;
    }
}