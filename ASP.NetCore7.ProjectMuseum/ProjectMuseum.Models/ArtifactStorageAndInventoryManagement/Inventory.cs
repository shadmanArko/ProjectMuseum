namespace ProjectMuseum.Models;

public class Inventory
{
    public List<InventoryItem> InventoryItems { get; set; }
    public List<Artifact> Artifacts { get; set; }
    public List<int> OccupiedSlots { get; set; }
    public int SlotsUnlocked { get; set; }
}