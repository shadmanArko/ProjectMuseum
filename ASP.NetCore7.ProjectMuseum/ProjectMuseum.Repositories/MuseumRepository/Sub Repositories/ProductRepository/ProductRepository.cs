using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.ProductRepository;

public class ProductRepository: IProductRepository
{
    private readonly JsonFileDatabase<Product> _productDatabase;

    public ProductRepository(JsonFileDatabase<Product> productDatabase)
    {
        _productDatabase = productDatabase;
    }
    public async Task<List<Product>?> GetAllProducts()
    {
        var datas = await _productDatabase.ReadDataAsync();
        return datas;
    }
}