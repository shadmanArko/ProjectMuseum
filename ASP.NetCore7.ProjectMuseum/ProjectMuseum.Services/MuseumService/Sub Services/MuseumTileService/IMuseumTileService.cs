using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IMuseumTileService
{
    Task<MuseumTile> InsertMuseumTile(MuseumTile museumTile);
    Task<MuseumTile?> GetMuseumTileById(string tileId);
    Task<List<MuseumTile>?> GetAllMuseumTiles();
    Task<List<ExhibitPlacementConditionData>> GetEligibilityOfPositioningExhibit(string exhibitType);
    Task<bool> PlaceExhibitOnTile(string tileId, string exhibitVariationName);
    Task<TilesWithExhibitDto> PlaceExhibitOnTiles(string originTileId, List<string> tileIds,
        string exhibitVariationName, int rotationFrame);
    Task<List<MuseumTile>> PlaceShopOnTiles(string originTileId, List<string> tileIds,
        string shopVariationName, int rotationFrame);
    Task<List<MuseumTile>> PlaceSanitationOnTiles(string originTileId, List<string> tileIds,
        string sanitationVariationName, int rotationFrame);
    Task<List<MuseumTile>> PlaceOtherDecorationOnTiles(string originTileId, List<string> tileIds,
        string otherVariationName, int rotationFrame);
    Task<MuseumTile> UpdateMuseumTileById(string tileId, MuseumTile museumTile);
    Task<List<MuseumTile>?> UpdateMuseumTilesSourceId(List<string> museumTilesId, int sourceId);
    Task<List<MuseumTile>?> UpdateMuseumTilesWallId(List<string> museumTilesId, string wallId);
    Task<MuseumTile?> DeleteMuseumTileById(string tileId);
    Task<List<MuseumTile>?> DeleteAllMuseumTiles();
    Task<List<MuseumTile>?> GenerateMuseumTileForNewGame();
    Task<List<MuseumTile>?> ExpandMuseumTiles(int originPositionX, int originPositionY);

}