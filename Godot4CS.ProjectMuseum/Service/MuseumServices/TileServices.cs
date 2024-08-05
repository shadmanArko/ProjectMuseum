using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class TileServices: Node
{
    private List<MuseumTile> _museumTileDatabase;

    public override void _Ready()
    {
        base._Ready();
        var museumTileDatabaseJson = File.ReadAllText(
            "E:/Godot Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Dummy Data Folder/museumTile.json");
        _museumTileDatabase = JsonSerializer.Deserialize<List<MuseumTile>>(museumTileDatabaseJson);
        GD.Print($"Got Museum tiles {_museumTileDatabase.Count}");
    }

    public async Task<MuseumTile> Insert(MuseumTile museumTile)
    {
        var museumTiles = _museumTileDatabase;
        museumTiles?.Add(museumTile);
        return museumTile;
    }

    public async Task<MuseumTile> Update(string id, MuseumTile museumTile)
    {
        var museumTiles = _museumTileDatabase;
        var museumTileToUpdate = museumTile;
        museumTileToUpdate.Id = id;
        return museumTile;
    }
    public List<MuseumTile> UpdateMuseumTilesSourceId(List<string> ids, int sourceId)
    {
        var museumTiles = _museumTileDatabase;
        
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
        
        return museumTiles;
    }

    public List<MuseumTile> UpdateMuseumTilesWallId(List<TileWallInfo> tileWallInfos)
    {
        var museumTiles = _museumTileDatabase;
        
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
        GD.Print($"Got return tiles {museumTiles.Count}");

        return museumTiles;
    }

    public async Task<MuseumTile?> GetById(string id)
    {
        var museumTiles = _museumTileDatabase;
        var museumTile = museumTiles!.FirstOrDefault(tile => tile.Id == id);
        return museumTile;
    }

    public async Task<MuseumTile?> GetByPosition(int xPosition, int yPosition)
    {
        var museumTiles = _museumTileDatabase;
        var museumTile = museumTiles!.FirstOrDefault(tile => tile.XPosition == xPosition && tile.YPosition == yPosition);
        return museumTile;
    }

    public List<MuseumTile> GetAll()
    {
        var museumTiles = _museumTileDatabase;
        return museumTiles;
    }
    public List<MuseumTile> ResetWalls()
    {
        var museumTiles = _museumTileDatabase;
        foreach (var museumTile in museumTiles)
        {
            museumTile.BackLeftWallId = "";
            museumTile.BackRightWallId = "";
            museumTile.FrontLeftWallId = "";
            museumTile.FrontRightWallId = "";
        }
        return museumTiles;
    }

    public async Task<MuseumTile?> UpdateExhibitToMuseumTile(string tileId, string exhibitId)
    {
        var museumTiles = _museumTileDatabase;
        var museumTile = museumTiles!.FirstOrDefault(tile => tile.Id == tileId);
        if (museumTile != null) museumTile.ItemId = exhibitId;
        if (museumTile != null) museumTile.Walkable = false; 
        return museumTile;
    }
    public List<MuseumTile> UpdateExhibitToMuseumTiles(List<string> tileIds, string exhibitId)
    {
        var museumTiles = _museumTileDatabase;
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
        return museumTiles;
    }
    public List<MuseumTile> UpdateMovedItemToMuseumTiles(List<string> previousTileIds, List<string> newTileIds, string itemId, ItemTypeEnum itemType)
    {
        var museumTiles = _museumTileDatabase;
        foreach (var tile in museumTiles)
        {
            if (newTileIds.Contains(tile.Id))
            {
                tile.ItemId = itemId;
                tile.Walkable = false;
                tile.ItemType = itemType;
                if (itemType == ItemTypeEnum.Exhibit)
                {
                    tile.HasExhibit = true;
                }
            }
            else if (previousTileIds.Contains(tile.Id))
            {
                tile.ItemId = "";
                tile.Walkable = true;
                tile.HasExhibit = false;
                tile.ItemType = ItemTypeEnum.None;
            }
            
        }
        return _museumTileDatabase;
    }
    public List<MuseumTile> UpdateShopToMuseumTiles(List<string> tileIds, string shopId)
    {
        var museumTiles = _museumTileDatabase;
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = shopId;
                tile.Walkable = false;
                tile.ItemType = ItemTypeEnum.Shop;
            }
        }
        return museumTiles;
    }
    public List<MuseumTile> UpdateSanitationToMuseumTiles(List<string> tileIds, string sanitationId)
    {
        var museumTiles = _museumTileDatabase;
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = sanitationId;
                tile.Walkable = false;
                tile.ItemType = ItemTypeEnum.Sanitation;
            }
        }
        return museumTiles;
    }
    public List<MuseumTile> UpdateOtherDecorationToMuseumTiles(List<string> tileIds, string decorationId)
    {
        var museumTiles = _museumTileDatabase;
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = decorationId;
                tile.Walkable = false;
                tile.ItemType = ItemTypeEnum.Decoration;
            }
        }
        return museumTiles;
    }
    public List<MuseumTile> RemoveItemFromMuseumTiles(List<string> tileIds)
    {
        var museumTiles = _museumTileDatabase;
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = "";
                tile.Walkable = true;
                tile.ItemType = ItemTypeEnum.None;
            }
        }
        return museumTiles;
    }

    public MuseumTile Delete(string id)
    {
        var museumTiles = _museumTileDatabase;
        var museumTile = museumTiles!.FirstOrDefault(tile => tile.Id == id);
        if (museumTile != null) museumTiles?.Remove(museumTile);
        return museumTile;
    }
    public List<MuseumTile> DeleteAll()
    {
        var museumTiles = new List<MuseumTile>();
        return museumTiles;
    }
}