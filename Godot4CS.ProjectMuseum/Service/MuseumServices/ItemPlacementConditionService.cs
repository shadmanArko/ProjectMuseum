using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.GameData;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Models.CoreShop;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class ItemPlacementConditionService: Node
{
    private MuseumRunningDataContainer _museumRunningDataContainer;
    private MuseumGameData _museumGameData;
    public  override async void _Ready()
    {
        base._Ready();

        InitializeData();
    }

    private void InitializeData()
    {
        _museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
        _museumGameData = ServiceRegistry.Resolve<MuseumGameData>();
        _museumRunningDataContainer.Shops = SaveLoadService.Load().Shops;
        _museumRunningDataContainer.Products = SaveLoadService.Load().Products;
        _museumRunningDataContainer.Sanitations = SaveLoadService.Load().Sanitations;
        _museumRunningDataContainer.DecorationOthers = SaveLoadService.Load().DecorationOthers;
    }

    public List<ExhibitPlacementConditionData> CanExhibitBePlacedOnThisTile(string exhibitVariationName)
    {
        InitializeData();
        var listOfExhibitPlacementConditionData = new List<ExhibitPlacementConditionData>();
        var museumTiles = _museumRunningDataContainer.MuseumTiles;
        

        foreach (var museumTile in museumTiles)
        {
            var exhibitTilePlacementData = new ExhibitPlacementConditionData();
            exhibitTilePlacementData.Id = museumTile.Id;
            exhibitTilePlacementData.TileXPosition = museumTile.XPosition;
            exhibitTilePlacementData.TileYPosition = museumTile.YPosition;
            exhibitTilePlacementData.IsEligible = museumTile.Walkable;
            listOfExhibitPlacementConditionData.Add(exhibitTilePlacementData);
        }
        

        return listOfExhibitPlacementConditionData;
    }
    public TilesWithExhibitDto PlaceExhibitOnTiles(string originTileId, List<string> tileIds, string exhibitVariationName, int rotationFrame)
    {
        TilesWithExhibitDto tilesWithExhibitDto = new TilesWithExhibitDto();
        Exhibit exhibit = new Exhibit();
        var museumTiles = _museumRunningDataContainer.MuseumTiles;
        tilesWithExhibitDto.Exhibit = exhibit;
        tilesWithExhibitDto.MuseumTiles = museumTiles;
        List<GridSlots2X2> artifactGridSlots2X2S = new List<GridSlots2X2>();
        if (tileIds.Count > 1)
        {
            artifactGridSlots2X2S = new List<GridSlots2X2>(4);
        }
        foreach (var tileId in tileIds)
        {
            if (tileId == originTileId)
            {
                var museumTile = museumTiles.FirstOrDefault(tile => tile.Id == tileId);
                if (museumTile != null && !museumTile.Walkable) return tilesWithExhibitDto;
                if (museumTile == null) return tilesWithExhibitDto;
                exhibit = new Exhibit
                {
                    Id = Guid.NewGuid().ToString(),
                    ExhibitVariationName = exhibitVariationName,
                    XPosition = museumTile.XPosition,
                    YPosition = museumTile.YPosition,
                    OccupiedTileIds = tileIds,
                    ArtifactIds = new List<string>(),
                    RotationFrame = rotationFrame,
                    ArtifactGridSlots2X2s = artifactGridSlots2X2S,
                    ExhibitDecoration = "string",
                    ExhibitArtifactSlot1 = "string",
                    ExhibitArtifactSlot2 = "string",
                    ExhibitArtifactSlot3 = "string",
                    ExhibitArtifactSlot4 = "string",
                    ExhibitArtifactSlot5 = "string"
                };
                _museumRunningDataContainer.Exhibits.Add(exhibit);
                
            }
        }
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = exhibit.Id;
                tile.Walkable = false;
                tile.HasExhibit = true;
                tile.ItemType = ItemTypeEnum.Exhibit;
            }
        }
        
        tilesWithExhibitDto.Exhibit = exhibit;
        var exhibits = _museumRunningDataContainer.Exhibits;
        tilesWithExhibitDto.Exhibits = exhibits;
        if (museumTiles != null) tilesWithExhibitDto.MuseumTiles = museumTiles;
        return tilesWithExhibitDto;
    }
    public TilesWithShopsDTO PlaceShopOnTiles(string originTileId, List<string> tileIds, string shopVariationName, int rotationFrame)
    {
        TilesWithShopsDTO tilesWithShopsDto = new TilesWithShopsDTO();
        Shop shop = new Shop();
        var museumTiles = _museumRunningDataContainer.MuseumTiles;
        //get core shop  functional with the variation name from json
        var coreShopFunctional = new CoreShopFunctional();
        var coreShopDescriptive = new CoreShopDescriptive();
        //
        var shopFunctional = Godot.FileAccess.Open(
            "res://Game Data/CoreShopData/coreShopFunctionalData.json", 
            Godot.FileAccess.ModeFlags.Read).GetAsText();

        var shopDescriptives = Godot.FileAccess.Open(
            "res://Game Data/CoreShopData/coreShopDescriptiveData.json", 
            Godot.FileAccess.ModeFlags.Read).GetAsText();

        var shopFunctionalDatas = JsonSerializer.Deserialize<List<CoreShopFunctional>>(shopFunctional);
        var shopDescriptiveDatas = JsonSerializer.Deserialize<List<CoreShopDescriptive>>(shopDescriptives);
        _museumGameData.CoreShopDescriptives = shopDescriptiveDatas;
        _museumGameData.CoreShopFunctionals = shopFunctionalDatas;
        foreach (var shopFunctionalData in shopFunctionalDatas)
        {
            if (shopFunctionalData.Variant == shopVariationName)
            {
                coreShopFunctional = shopFunctionalData;
            }
        }
        foreach (var shopDescriptive in shopDescriptiveDatas)
        {
            if (shopDescriptive.Variant == shopVariationName)
            {
                coreShopDescriptive = shopDescriptive;
            }
        }
        
        foreach (var tileId in tileIds)
        {
            if (tileId == originTileId)
            {
                var museumTile = museumTiles.FirstOrDefault(tile => tile.Id == tileId);
                shop = new Shop
                {
                    Id = Guid.NewGuid().ToString(),
                    CoreShopFunctional =  coreShopFunctional,
                    CoreShopDescriptive =  coreShopDescriptive,
                    XPosition = museumTile.XPosition,
                    YPosition = museumTile.YPosition,
                    RotationFrame = rotationFrame,
                    
                };
                foreach (var defaultProduct in coreShopFunctional.DefaultProducts)
                {
                    defaultProduct.Id = Guid.NewGuid().ToString();
                    defaultProduct.ShopId = shop.Id;
                }
                _museumRunningDataContainer.Shops.Add(shop);
                
            }
        }
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = shop.Id;
                tile.Walkable = false;
                tile.ItemType = ItemTypeEnum.Shop;
            }
        }
        tilesWithShopsDto.MuseumTiles = museumTiles;
        tilesWithShopsDto.DecorationShops = _museumRunningDataContainer.Shops;
        tilesWithShopsDto.Shop = shop;
        return tilesWithShopsDto;
    }
    public TilesWithSanitationsDTO PlaceSanitationOnTiles(string originTileId, List<string> tileIds, string sanitationVariationName, int rotationFrame)
    {
        TilesWithSanitationsDTO tilesWithSanitationsDto = new TilesWithSanitationsDTO();
        Sanitation sanitation = new Sanitation();
        var museumTiles = _museumRunningDataContainer.MuseumTiles;
        
        foreach (var tileId in tileIds)
        {
            if (tileId == originTileId)
            {
                var museumTile = _museumRunningDataContainer.MuseumTiles.FirstOrDefault();
                sanitation = new Sanitation()
                {
                    Id = Guid.NewGuid().ToString(),
                    SanitationVariationName =  sanitationVariationName,
                    XPosition = museumTile.XPosition,
                    YPosition = museumTile.YPosition,
                    RotationFrame = rotationFrame,
                    
                };
                _museumRunningDataContainer.Sanitations.Add(sanitation);
                
            }
        }
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = sanitation.Id;
                tile.Walkable = false;
                tile.ItemType = ItemTypeEnum.Sanitation;
            }
        }
        tilesWithSanitationsDto.MuseumTiles = museumTiles;
        tilesWithSanitationsDto.Sanitations = _museumRunningDataContainer.Sanitations;
        tilesWithSanitationsDto.Sanitation = sanitation;
        return tilesWithSanitationsDto;
    }
    public List<MuseumTile> PlaceOtherDecorationOnTiles(string originTileId, List<string> tileIds, string otherVariationName, int rotationFrame)
    {
        
        DecorationOther decorationOther = new DecorationOther();
        var museumTiles = _museumRunningDataContainer.MuseumTiles;
        
        foreach (var tileId in tileIds)
        {
            if (tileId == originTileId)
            {
                var museumTile = museumTiles.FirstOrDefault(tile => tile.Id == tileId);
                decorationOther = new DecorationOther
                {
                    Id = Guid.NewGuid().ToString(),
                    VariationName =  otherVariationName,
                    XPosition = museumTile.XPosition,
                    YPosition = museumTile.YPosition,
                    RotationFrame = rotationFrame,
                    
                };
                _museumRunningDataContainer.DecorationOthers.Add(decorationOther);
                
            }
        }
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = decorationOther.Id;
                tile.Walkable = false;
                tile.ItemType = ItemTypeEnum.Decoration;
            }
        }
        return museumTiles;
    }

}