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
    Task<Exhibit> PlaceExhibitOnTiles(string originTileId, List<string> tileIds, string exhibitVariationName);

    Task<MuseumTile> UpdateMuseumTileById(string tileId, MuseumTile museumTile);
    Task<List<MuseumTile>?> UpdateMuseumTilesSourceId(List<string> museumTilesId, int sourceId);
    Task<MuseumTile?> DeleteMuseumTileById(string tileId);
    Task<List<MuseumTile>?> DeleteAllMuseumTiles();
    Task<List<MuseumTile>?> GenerateMuseumTileForNewGame();

}