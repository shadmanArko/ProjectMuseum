namespace ProjectMuseum.Models.Vehicles;

public class ElevatorPlatform
{
    public string Id { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    
    public string SmallPngPath { get; set; }
    public string LargePngPath { get; set; }
    
    public float Speed { get; set; }
    public bool CanMove { get; set; }
    public bool IsMoving { get; set; }
}