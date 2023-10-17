namespace ProjectMuseum.Models;

public class ExhibitPlacementConditionData
{
    public string Id { get; set; }
    public int TileXPosition { get; set; }
    public int TileYPosition { get; set; }
    public bool IsEligible { get; set; }
}