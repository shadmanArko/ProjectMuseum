using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IMuseumTileDataGenerator
{
    Task<List<MuseumTile>?> GenerateMuseumTileDataForNewMuseum();
}