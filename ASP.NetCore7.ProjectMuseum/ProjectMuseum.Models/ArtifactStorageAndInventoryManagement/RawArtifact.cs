namespace ProjectMuseum.Models;

public class RawArtifact
{
    public string Id { get; set; }
    public string ArtifactName { get; set; }
    public string Era { get; set; }
    public string Region { get; set; }
    public string Object { get; set; }
    public string Material { get; set; }
    public string ObjectClass { get; set; }
    public string ObjectSize { get; set; }
    public string ImageLocation { get; set; }

    public string Description { get; set; }
    //todo rest of the field will be coded here
}