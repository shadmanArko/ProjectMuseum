using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IMuseumTileService
{
    Task<MuseumTile> InsertMuseumTile(MuseumTileDto museumTileDto);
    Task<MuseumTile> GetMuseumTileById(string tileId);
    Task<List<MuseumTileDto>> GetAllMuseumTiles();
    Task<MuseumTile> DeleteMuseumTileById();
}