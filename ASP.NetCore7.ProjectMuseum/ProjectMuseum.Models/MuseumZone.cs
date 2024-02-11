namespace ProjectMuseum.Models;

public class MuseumZone
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public List<string> OccupiedMuseumTiles { get; set; }
}