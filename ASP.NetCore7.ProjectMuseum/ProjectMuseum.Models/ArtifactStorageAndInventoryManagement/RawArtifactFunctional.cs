namespace ProjectMuseum.Models;

public class RawArtifactFunctional
{
    public string Id { get; set; }
    public string Era { get; set; }
    public string Region { get; set; }
    public string Object { get; set; }
    public List<string> Materials { get; set; }
    public string ObjectClass { get; set; }
    public string ObjectSize { get; set; }
    public string ImageLocation { get; set; }
}