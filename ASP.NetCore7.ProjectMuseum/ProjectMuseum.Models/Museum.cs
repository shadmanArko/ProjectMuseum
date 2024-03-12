namespace ProjectMuseum.Models;

public class Museum
{
    public string Id { get; set; }
    public string Name { get; set; }
    public float Money { get; set; }
    public MuseumTicketCounter MuseumTicketCounter { get; set; }
    public List<MuseumZone> MuseumZones { get; set; }
}