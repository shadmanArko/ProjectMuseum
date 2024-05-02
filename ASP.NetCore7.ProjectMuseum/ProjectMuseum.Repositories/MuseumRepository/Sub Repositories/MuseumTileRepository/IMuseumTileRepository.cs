using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumTileRepository;

public interface IMuseumTileRepository
{
    Task<MuseumTile> Insert(MuseumTile museumTile);
    Task<MuseumTile> Update(string id, MuseumTile museumTile);
    Task<MuseumTile?> GetById(string id);
    Task<MuseumTile?> GetByPosition(int xPosition, int yPosition);
    Task<List<MuseumTile>?> GetAll();
    Task<List<MuseumTile>?> ResetWalls();
    Task<List<MuseumTile>?> UpdateMuseumTilesSourceId(List<string> ids, int sourceId);
    Task<List<MuseumTile>?> UpdateMuseumTilesWallId(List<TileWallInfo> tileWallInfos);
    Task<MuseumTile?> UpdateExhibitToMuseumTile(string tileId, string exhibitId);
    Task<List<MuseumTile>?> UpdateExhibitToMuseumTiles(List<string> tileIds, string exhibitId);
    Task<List<MuseumTile>?> UpdateShopToMuseumTiles(List<string> tileIds, string shopId);
    Task<List<MuseumTile>?> UpdateSanitationToMuseumTiles(List<string> tileIds, string shopId);
    Task<List<MuseumTile>?> UpdateOtherDecorationToMuseumTiles(List<string> tileIds, string shopId);
    Task<MuseumTile?> Delete(string id);
    Task<List<MuseumTile>
        ?> DeleteAll();
}