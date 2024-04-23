namespace ProjectMuseum.Models;

public class GuestBuildingParameter
{
    public List<string> AllAvailableArtifactTags { get; set; }
    public MinMaxFloat GuestMoneyRange { get; set; }

    public MinMaxFloat HungerLevelRange { get; set; }
    public MinMaxFloat ThirstLevelRange { get; set; }
    public MinMaxFloat ChargeLevelRange { get; set; }
    public MinMaxFloat BladderLevelRange { get; set; }
    public MinMaxFloat EnergyLevelRange { get; set; }
    public MinMaxFloat InterestInArtifactLevelRange { get; set; }
    public MinMaxFloat EntertainmentLevelRange { get; set; }
    
    public MinMaxFloat HungerDecayRange { get; set; }
    public MinMaxFloat ThirstDecayRange { get; set; }
    public MinMaxFloat ChargeDecayRange { get; set; }
    public MinMaxFloat BladderDecayRange { get; set; }
    public MinMaxFloat EnergyDecayRange { get; set; }
    public MinMaxFloat InterestInArtifactDecayRange { get; set; }
    public MinMaxFloat EntertainmentDecayRange { get; set; }
}