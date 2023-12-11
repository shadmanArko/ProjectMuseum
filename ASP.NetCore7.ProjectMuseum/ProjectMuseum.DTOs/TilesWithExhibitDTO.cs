using ProjectMuseum.Models;

namespace ProjectMuseum.DTOs;

public class TilesWithExhibitDto
{
    public List<MuseumTile> MuseumTiles { get; set; }
    public Exhibit Exhibit { get; set; }
}