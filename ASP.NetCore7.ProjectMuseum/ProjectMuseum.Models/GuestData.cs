namespace ProjectMuseum.Models;

public class GuestData
{
    public string Name { get; set; }
    public int Age { get; set; }
    public float Money { get; set; }
    public List<string> InterestedTags { get; set; }
    public float HungerDecayRate { get; set; }
    public float ThirstDecayRate { get; set; }
    public float ChargeDecayRate { get; set; }
    public float BladderDecayRate { get; set; }
    public float EnergyDecayRate { get; set; }
    public float InterestInArtifactDecayRate { get; set; }
    public float EntertainmentDecayRate { get; set; }
}