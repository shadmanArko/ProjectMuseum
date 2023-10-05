using AutoMapper;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumTileService;

public class MuseumTileService : IMuseumTileService
{
    private readonly IMuseumTileRepository _museumTileRepository;

    public MuseumTileService(IMuseumTileRepository museumTileRepository)
    {
        _museumTileRepository = museumTileRepository;
    }

    public Task<MuseumTile> InsertMuseumTile(MuseumTileDto museumTileDto)
    {
        throw new NotImplementedException();
    }

    public Task<MuseumTile> GetMuseumTileById(string tileId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<MuseumTileDto>> GetAllMuseumTiles()
    {
        var museumTileDtos = await _museumTileRepository.GetAll();
        return museumTileDtos;
    }

    public Task<MuseumTile> DeleteMuseumTileById()
    {
        throw new NotImplementedException();
    }
}