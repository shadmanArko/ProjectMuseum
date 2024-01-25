namespace ProjectMuseum.Models;

public class Inventory
{
    public List<InventoryItem> Equipables { get; set; }
    public List<Artifact> Artifacts { get; set; }
    public List<int> EmptySlots { get; set; }
    public int SlotsUnlocked { get; set; }
}