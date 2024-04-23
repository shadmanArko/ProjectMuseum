using ProjectMuseum.Models;

namespace ProjectMuseum.DTOs;

public class TilesWithSanitationsDTO
{
    public List<MuseumTile> MuseumTiles { get; set; }
    public List<Sanitation>? Sanitations { get; set; }
    public Sanitation Sanitation { get; set; }
}