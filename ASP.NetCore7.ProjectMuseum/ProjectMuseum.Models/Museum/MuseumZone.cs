namespace ProjectMuseum.Models;

public class MuseumZone
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Color { get; set; }
    public float ThemeValue { get; set; }
    public List<string> OccupiedMuseumTileIds { get; set; }
}