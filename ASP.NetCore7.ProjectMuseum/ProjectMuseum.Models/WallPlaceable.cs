namespace ProjectMuseum.Models;

public class WallPlaceable
{
    public string Id { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public List<string> OccupiedCellIds { get; set; }
    public int ExtraOccupiedDimensionX { get; set; }
    public int ExtraOccupiedDimensionY { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public string Variant { get; set; }
    public string ScenePath { get; set; }
}