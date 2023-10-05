using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IMuseumTileService
{
    Task<MuseumTileDto> InsertMuseumTile(MuseumTileDto museumTileDto);
    Task<MuseumTileDto?> GetMuseumTileById(string tileId);
    Task<List<MuseumTileDto>> GetAllMuseumTiles();
    Task<MuseumTile> DeleteMuseumTileById();
}