using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumTileRepository;

public interface IMuseumTileRepository
{
    Task<MuseumTile> Insert(MuseumTile museumTile);
    Task<MuseumTile> Update(MuseumTile museumTile);
    Task<MuseumTile> GetById(string id);
    Task<List<MuseumTileDto>> GetAll();
    Task<MuseumTile> Delete();
}