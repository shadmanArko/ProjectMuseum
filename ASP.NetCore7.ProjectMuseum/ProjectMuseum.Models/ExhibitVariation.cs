namespace ProjectMuseum.Models;

public class ExhibitVariation
{
    public string VariationName { get; set; }
    public float Price { get; set; }
    public string ExhibitDecoration { get; set; }
    public string ExhibitSize { get; set; }
    public int NumberOfTilesNeeded { get; set; }
    public string TilesExtendInDirection { get; set; }
    public bool IsHangingExhibit { get; set; }
    public bool IsWallExhibit { get; set; }
}