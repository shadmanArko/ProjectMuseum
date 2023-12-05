using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumTileRepository;

public interface IMuseumTileRepository
{
    Task<MuseumTile> Insert(MuseumTile museumTile);
    Task<MuseumTile> Update(string id, MuseumTile museumTile);
    Task<MuseumTile?> GetById(string id);
    Task<MuseumTile?> GetByPosition(int xPosition, int yPosition);
    Task<List<MuseumTile>?> GetAll();
    Task<List<MuseumTile>?> UpdateMuseumTilesSourceId(List<string> ids, int sourceId);
    Task<MuseumTile?> UpdateExhibitToMuseumTile(string tileId, string exhibitId);
    Task<List<MuseumTile>?> UpdateExhibitToMuseumTiles(List<string> tileIds, string exhibitId);
    Task<MuseumTile?> Delete(string id);
    Task<List<MuseumTile>
        ?> DeleteAll();
}