namespace ProjectMuseum.Models;

public class Equipable
{
    public string Id { get; set; }
    public string Name { get; set; }
    
    public int Slot { get; set; }
    public string ItemType { get; set; }
    public string ItemSubcategory { get; set; }
    public bool IsStackable { get; set; }
    public int StackNo { get; set; }
    public string SmallPngPath { get; set; }
}