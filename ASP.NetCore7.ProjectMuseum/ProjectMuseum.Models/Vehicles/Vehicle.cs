namespace ProjectMuseum.Models.Vehicles;

public class Vehicle : Item
{
    public float Speed { get; set; }
    public bool CanMove { get; set; }
    public bool IsMoving { get; set; }
}