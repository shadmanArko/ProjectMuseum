namespace ProjectMuseum.Models.Vehicles;

public class VehicleStat : Vehicle
{
    public float Speed { get; set; }
    public bool CanMove { get; set; }
    public bool IsMoving { get; set; }
    
    public int PositionX { get; set; }
    public int PositionY { get; set; }
}