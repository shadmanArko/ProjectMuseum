using ProjectMuseum.Models;

namespace ProjectMuseum.DTOs;

public class TilesWithShopsDTO
{
    public List<MuseumTile> MuseumTiles { get; set; }
    public List<Shop>? DecorationShops { get; set; }
    public Shop Shop { get; set; }
}