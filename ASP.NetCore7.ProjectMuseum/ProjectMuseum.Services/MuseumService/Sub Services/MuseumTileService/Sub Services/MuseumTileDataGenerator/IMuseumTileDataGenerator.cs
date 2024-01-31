using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IMuseumTileDataGenerator
{
    Task<List<MuseumTile>?> GenerateMuseumTileDataForNewMuseum();
    Task<List<MuseumTile>?> ExpandMuseumTiles(int originPositionX, int originPositionY);
}