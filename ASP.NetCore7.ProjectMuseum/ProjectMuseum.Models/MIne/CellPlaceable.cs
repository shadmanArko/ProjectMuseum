namespace ProjectMuseum.Models;

public class CellPlaceable
{
    public string Id { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public string Name { get; set; }
    public string OccupiedCellId { get; set; }
    public int ExtraOccupiedDimensionX { get; set; }
    public int ExtraOccupiedDimensionY { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public string Variant { get; set; }
    public string ScenePath { get; set; }
    public string PngPath { get; set; }
}