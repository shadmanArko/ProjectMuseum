using AutoMapper;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumTileRepository;

public class MuseumTileRepository : IMuseumTileRepository
{
    private readonly JsonFileDatabase<MuseumTile> _museumTileDatabase;
    private readonly IMapper _mapper;

    public MuseumTileRepository(JsonFileDatabase<MuseumTile> museumTileDatabase, IMapper mapper)
    {
        _museumTileDatabase = museumTileDatabase;
        _mapper = mapper;
    }

    public Task<MuseumTile> Insert(MuseumTile museumTile)
    {
        throw new NotImplementedException();
    }

    public Task<MuseumTile> Update(MuseumTile museumTile)
    {
        throw new NotImplementedException();
    }

    public Task<MuseumTile> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<MuseumTileDto>> GetAll()
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        var museumTileDtos = _mapper.Map<List<MuseumTileDto>>(museumTiles);
        return museumTileDtos;
    }

    public Task<MuseumTile> Delete()
    {
        throw new NotImplementedException();
    }
}