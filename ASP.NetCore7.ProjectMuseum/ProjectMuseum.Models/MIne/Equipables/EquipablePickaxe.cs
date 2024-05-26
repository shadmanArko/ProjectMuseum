namespace ProjectMuseum.Models.MIne.Equipables;

public class EquipablePickaxe
{
    public string Id { get; set; }
    public string Name { get; set; }
    
    public string Type { get; set; }
    public string Category { get; set; }
    public string Variant { get; set; }
    
    public string AnimantionSpriteSheetPath { get; set; }
    public int Damage { get; set; }
    public float Cooldown { get; set; }
    public bool HasStatusEffect { get; set; }
    public List<string> ListOfStatusEffects { get; set; }
}