using AutoMapper;
using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumTileRepository;

public class MuseumTileRepository : IMuseumTileRepository
{
    private readonly JsonFileDatabase<MuseumTile> _museumTileDatabase;

    public MuseumTileRepository(JsonFileDatabase<MuseumTile> museumTileDatabase)
    {
        _museumTileDatabase = museumTileDatabase;
    }

    public async Task<MuseumTile> Insert(MuseumTile museumTile)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        museumTiles?.Add(museumTile);
        if (museumTiles != null) await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTile;
    }

    public async Task<MuseumTile> Update(string id, MuseumTile museumTile)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        var museumTileToUpdate = museumTile;
        museumTileToUpdate.Id = id;
        if (museumTiles != null) await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTile;
    }
    public async Task<List<MuseumTile>?> UpdateMuseumTilesSourceId(List<string> ids, int sourceId)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        
        foreach (var id in ids)
        {
            if (museumTiles != null)
                foreach (var museumTile in museumTiles)
                {
                    if (museumTile.Id == id)
                    {
                        museumTile.TileSetNumber = sourceId;
                    }
                }
        }
        
        if (museumTiles != null) await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTiles;
    }

    public async Task<List<MuseumTile>?> UpdateMuseumTilesWallId(List<string> ids, string wallId)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        
        foreach (var id in ids)
        {
            if (museumTiles != null)
                foreach (var museumTile in museumTiles)
                {
                    if (museumTile.Id == id)
                    {
                        museumTile.WallId = wallId;
                    }
                }
        }
        
        if (museumTiles != null) await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTiles;
    }

    public async Task<MuseumTile?> GetById(string id)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        var museumTile = museumTiles!.FirstOrDefault(tile => tile.Id == id);
        return museumTile;
    }

    public async Task<MuseumTile?> GetByPosition(int xPosition, int yPosition)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        var museumTile = museumTiles!.FirstOrDefault(tile => tile.XPosition == xPosition && tile.YPosition == yPosition);
        return museumTile;
    }

    public async Task<List<MuseumTile>?> GetAll()
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        return museumTiles;
    }

    public async Task<MuseumTile?> UpdateExhibitToMuseumTile(string tileId, string exhibitId)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        var museumTile = museumTiles!.FirstOrDefault(tile => tile.Id == tileId);
        if (museumTile != null) museumTile.ExhibitId = exhibitId;
        if (museumTile != null) museumTile.Walkable = false;
        if (museumTiles != null) await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTile;
    }
    public async Task<List<MuseumTile>?> UpdateExhibitToMuseumTiles(List<string> tileIds, string exhibitId)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ExhibitId = exhibitId;
                tile.Walkable = false;
            }
        }
        await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTiles;
    }

    public async Task<MuseumTile?> Delete(string id)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        var museumTile = museumTiles!.FirstOrDefault(tile => tile.Id == id);
        if (museumTile != null) museumTiles?.Remove(museumTile);
        if (museumTiles != null) await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTile;
    }
    public async Task<List<MuseumTile>?> DeleteAll()
    {
        var museumTiles = new List<MuseumTile>();
        await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTiles;
    }
}