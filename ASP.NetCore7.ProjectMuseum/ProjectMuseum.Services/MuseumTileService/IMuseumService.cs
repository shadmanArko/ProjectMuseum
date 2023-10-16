using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IMuseumService
{
    Task<MuseumTile> InsertMuseumTile(MuseumTile museumTile);
    Task<MuseumTile?> GetMuseumTileById(string tileId);
    Task<List<MuseumTile>?> GetAllMuseumTiles();
    Task<bool> GetEligibilityOfPositioningExhibit(string exhibitType, int tileXPosition, int tileYPosition);
    Task<MuseumTile> UpdateMuseumTileById(string tileId, MuseumTile museumTile);
    Task<MuseumTile?> DeleteMuseumTileById(string tileId);
    Task<List<MuseumTile>?> GenerateMuseumTileForNewGame();
}