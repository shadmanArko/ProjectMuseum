namespace ProjectMuseum.Models;

public class InventoryItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public string Variant { get; set; }
    public bool IsStackable { get; set; }
    public int Stack { get; set; }
    public int Slot { get; set; }
    public string PngPath { get; set; }
}