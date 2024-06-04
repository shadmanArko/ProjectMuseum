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

    public async Task<List<MuseumTile>?> UpdateMuseumTilesWallId(List<TileWallInfo> tileWallInfos)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        
        foreach (var tile in tileWallInfos)
        {
            if (museumTiles != null)
                foreach (var museumTile in museumTiles)
                {
                    if (museumTile.Id == tile.TileId)
                    {
                        museumTile.BackLeftWallId = museumTile.BackLeftWallId.Length< 2? tile.BackLeftWallId:museumTile.BackLeftWallId;
                        museumTile.BackRightWallId = museumTile.BackRightWallId.Length< 2? tile.BackRightWallId:museumTile.BackRightWallId;
                        museumTile.FrontRightWallId = museumTile.FrontRightWallId.Length< 2? tile.FrontRightWallId:museumTile.FrontRightWallId;
                        museumTile.FrontLeftWallId = museumTile.FrontLeftWallId.Length< 2? tile.FrontLeftWallId:museumTile.FrontLeftWallId;
                        // museumTile.BackRightWallId = tile.BackRightWallId;
                        // museumTile.FrontLeftWallId = tile.FrontLeftWallId;
                        // museumTile.FrontRightWallId = tile.FrontRightWallId;
                    }
                }
        }
        
        if (museumTiles != null) await _museumTileDatabase.WriteDataAsync(museumTiles);
        return await _museumTileDatabase.ReadDataAsync();
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
    public async Task<List<MuseumTile>?> ResetWalls()
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        foreach (var museumTile in museumTiles)
        {
            museumTile.BackLeftWallId = "";
            museumTile.BackRightWallId = "";
            museumTile.FrontLeftWallId = "";
            museumTile.FrontRightWallId = "";
        }
        if (museumTiles != null) await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTiles;
    }

    public async Task<MuseumTile?> UpdateExhibitToMuseumTile(string tileId, string exhibitId)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        var museumTile = museumTiles!.FirstOrDefault(tile => tile.Id == tileId);
        if (museumTile != null) museumTile.ItemId = exhibitId;
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
                tile.ItemId = exhibitId;
                tile.Walkable = false;
                tile.HasExhibit = true;
                tile.ItemType = ItemTypeEnum.Exhibit;
            }
        }
        await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTiles;
    }
    public async Task<List<MuseumTile>?> UpdateMovedExhibitToMuseumTiles(List<string> previousTileIds, List<string> newTileIds, string exhibitId)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        foreach (var tile in museumTiles)
        {
            if (newTileIds.Contains(tile.Id))
            {
                tile.ItemId = exhibitId;
                tile.Walkable = false;
                tile.HasExhibit = true;
                tile.ItemType = ItemTypeEnum.Exhibit;
            }
            else if (previousTileIds.Contains(tile.Id))
            {
                tile.ItemId = "";
                tile.Walkable = true;
                tile.HasExhibit = false;
                tile.ItemType = ItemTypeEnum.None;
            }
        }
        await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTiles;
    }
    public async Task<List<MuseumTile>?> UpdateShopToMuseumTiles(List<string> tileIds, string shopId)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = shopId;
                tile.Walkable = false;
                tile.ItemType = ItemTypeEnum.Shop;
            }
        }
        await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTiles;
    }
    public async Task<List<MuseumTile>?> UpdateSanitationToMuseumTiles(List<string> tileIds, string sanitationId)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = sanitationId;
                tile.Walkable = false;
                tile.ItemType = ItemTypeEnum.Sanitation;
            }
        }
        await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTiles;
    }
    public async Task<List<MuseumTile>?> UpdateOtherDecorationToMuseumTiles(List<string> tileIds, string decorationId)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = decorationId;
                tile.Walkable = false;
                tile.ItemType = ItemTypeEnum.Decoration;
            }
        }
        await _museumTileDatabase.WriteDataAsync(museumTiles);
        return museumTiles;
    }
    public async Task<List<MuseumTile>?> RemoveItemFromMuseumTiles(List<string> tileIds)
    {
        var museumTiles = await _museumTileDatabase.ReadDataAsync();
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = "";
                tile.Walkable = true;
                tile.ItemType = ItemTypeEnum.None;
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