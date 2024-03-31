 namespace ProjectMuseum.Models;

public class MuseumTile
{
    public string Id { get; set; }
    public int XPosition { get; set; }
    public int YPosition { get; set; }
    public int TileSetNumber { get; set; }
    public int TileAtlasCoOrdinateX { get; set; }
    public int TileAtlasCoOrdinateY { get; set; }
    public int Layer { get; set; }
    public bool IsInZone { get; set; }
    public string WallId { get; set; }
    public bool Walkable { get; set; }
    public string ExhibitId { get; set; }
    public string ShopId { get; set; }
    public string DecorationId { get; set; }
    public string HangingLightId { get; set; }

}