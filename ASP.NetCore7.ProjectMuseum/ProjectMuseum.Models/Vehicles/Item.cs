namespace ProjectMuseum.Models.Vehicles;

public class Item
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public string SmallPngPath { get; set; }
    public string LargePngPath { get; set; }
}