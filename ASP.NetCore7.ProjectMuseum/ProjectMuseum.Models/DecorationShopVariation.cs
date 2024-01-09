namespace ProjectMuseum.Models;

public class DecorationShopVariation
{
    public string VariationName { get; set; }
    public int NumberOfTilesNeeded { get; set; }
    public bool IsDrinkShop { get; set; }
    public bool IsFoodShop { get; set; }
    public bool IsSouvenirShop { get; set; }
    public float BasePricePerItem { get; set; }
    public int LengthInTiles { get; set; }
    public int WidthInTiles { get; set; }
    public int BeautyRating { get; set; }
    public float PlacementCost { get; set; }
}