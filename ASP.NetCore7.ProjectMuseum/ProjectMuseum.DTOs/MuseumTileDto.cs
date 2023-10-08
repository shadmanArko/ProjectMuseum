namespace ProjectMuseum.DTOs;

public class MuseumTileDto
{
    //public string Id { get; set; }
    public int XPosition { get; set; }
    public int YPosition { get; set; }
    public int TileSetNumber { get; set; }
    public int TileAtlasCoOrdinateX { get; set; }
    public int TileAtlasCoOrdinateY { get; set; }
    public int Layer { get; set; }
    public string Flooring { get; set; }
    public string Decoration { get; set; }
}