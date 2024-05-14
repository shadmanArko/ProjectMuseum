using Godot4CS.ProjectMuseum.Scripts.Museum.GuestScripts;

namespace ProjectMuseum.Models.CoreShop;

public class CoreShopFunctional
{
    public string Id { get; set; }
    public string Variant { get; set; }
    public bool RequiredManpower { get; set; }
    public List<GuestNeedsEnum> NeedsShopFullfills { get; set; }
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