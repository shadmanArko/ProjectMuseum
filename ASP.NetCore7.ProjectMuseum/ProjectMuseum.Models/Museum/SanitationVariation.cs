namespace ProjectMuseum.Models;

public class SanitationVariation
{
    public string SanitationId { get; set; }
    public float PlacementCost { get; set; }
    public int CanAccomodate { get; set; }
    public int LengthInTiles { get; set; }
    public int WidthInTiles { get; set; }
    public int NumberOfTilesNeeded { get; set; }
    public int NumberOfFrames { get; set; }
    public float BeautyRating { get; set; }
}