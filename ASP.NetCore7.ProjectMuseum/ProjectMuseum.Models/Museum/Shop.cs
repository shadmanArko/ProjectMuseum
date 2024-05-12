namespace ProjectMuseum.Models;

public class Shop
{
    public string Id { get; set; }
    public string ShopVariant { get; set; }
    public int XPosition { get; set; }
    public int YPosition { get; set; }
    public int RotationFrame { get; set; }
    public bool RequiredManpower { get; set; }
    public int MaxProducts { get; set; }
    public string ProductCategoryTaken { get; set; }
    public List<Product> DefaultProducts { get; set; }
    public int LengthInTiles { get; set; }
    public int WidthInTiles { get; set; }
    public float BeautyRating { get; set; }
    public float PlacementCost { get; set; }
    public string FunctionalDirection { get; set; }
    public string ShopPngLocation { get; set; }
    
}