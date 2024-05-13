namespace ProjectMuseum.Models;

public class Product
{
    public string Id { get; set; }
    public string ShopId { get; set; }
    public string ProductVariant { get; set; }
    public string ProductCategory { get; set; }
    public string ProductType { get; set; }
    public int BasePrice { get; set; }
    public int NeedFillAmount { get; set; }
}