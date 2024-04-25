namespace ProjectMuseum.Models.MIne.Equippables;

public class EquippableMelee
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Variant { get; set; }
    public string AnimantionSpriteSheetPath { get; set; }
    public float Damage { get; set; }
    public float Cooldown { get; set; }
    public bool HasStatusAffect { get; set; }
    public List<string> ListOfStatusAffects { get; set; }
}