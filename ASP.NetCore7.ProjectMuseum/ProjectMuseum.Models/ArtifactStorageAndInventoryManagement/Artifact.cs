namespace ProjectMuseum.Models;

public class Artifact
{
    public string Id { get; set; }
    public string RawArtifactId { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int Slot { get; set; }

    public string Condition { get; set; }
    public string Rarity { get; set; }
    
    public string Era { get; set; }
    public string Region { get; set; }
    public string Object { get; set; }
    public string ObjectClass { get; set; }
    public string ObjectSize { get; set; }
    public List<string> Materials { get; set; }
    
    //todo rest of the field will be coded here
}