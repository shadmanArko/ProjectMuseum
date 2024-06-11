using ProjectMuseum.Models;

namespace ProjectMuseum.DTOs;

public class ExhibitWithNewTiles
{
    public Exhibit Exhibit { get; set; }
    public List<string> NewTileIds { get; set; }
}