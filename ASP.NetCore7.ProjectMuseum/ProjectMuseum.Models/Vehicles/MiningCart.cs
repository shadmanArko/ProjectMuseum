namespace ProjectMuseum.Models.Vehicles;

public class MiningCart : Vehicle
{
    public float Speed { get; set; }
    public bool CanMove { get; set; }
    public bool IsMoving { get; set; }
}