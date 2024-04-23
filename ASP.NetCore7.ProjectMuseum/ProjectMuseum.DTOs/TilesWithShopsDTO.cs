using ProjectMuseum.Models;

namespace ProjectMuseum.DTOs;

public class TilesWithShopsDTO
{
    public List<MuseumTile> MuseumTiles { get; set; }
    public List<DecorationShop>? DecorationShops { get; set; }
    public DecorationShop DecorationShop { get; set; }
}