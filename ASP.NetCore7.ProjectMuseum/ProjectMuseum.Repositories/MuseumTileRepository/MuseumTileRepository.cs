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

    public async Task<MuseumTile> Insert(MuseumTile museumTile)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        museumTiles?.Add(museumTile);
        if (museumTiles != null) await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTile;
    }

    public Task<MuseumTile> Update(MuseumTile museumTile)
    {
        throw new NotImplementedException();
    }

    public async Task<MuseumTile?> GetById(string id)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        var museumTile = museumTiles!.FirstOrDefault(tile => tile.Id == id);
        return museumTile;
    }

    public async Task<List<MuseumTile>?> GetAll()
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        return museumTiles;
    }

    public Task<MuseumTile> Delete()
    {
        throw new NotImplementedException();
    }
}