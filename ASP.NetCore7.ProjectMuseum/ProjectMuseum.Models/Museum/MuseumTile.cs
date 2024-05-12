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
    public string BackLeftWallId { get; set; }
    public string BackRightWallId { get; set; }
    public string FrontLeftWallId { get; set; }
    public string FrontRightWallId { get; set; }
    public bool Walkable { get; set; }
    public bool HasExhibit { get; set; }
    public string ItemId { get; set; }
    public ItemTypeEnum ItemType { get; set; }

}