namespace ProjectMuseum.Models.Vehicles;

public class ElevatorPlatform : Vehicle
{
    public float Speed { get; set; }
    public bool CanMove { get; set; }
    public bool IsMoving { get; set; }
}