namespace ProjectMuseum.Models;

public class Consumable
{
    public string Id { get; set; }
    
    public string Type { get; set; }
    public string Category { get; set; }
    public string Variant { get; set; }

    public int HealthEffectValue { get; set; }
    public int EnergyEffectValue { get; set; }
    public int MovementSpeedEffectValue { get; set; }
    public int AddedDamagePerHitEffectValue { get; set; }
    public int ClimbingSpeedEffectValue { get; set; }
    public int InvulnerabilityEffectValue { get; set; }
    
    public string ScenePath { get; set; }
    public string PngPath { get; set; }
}