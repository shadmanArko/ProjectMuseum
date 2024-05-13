using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories.DecorationOtherRepository;
using ProjectMuseum.Repositories.DecorationRepository;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.SanitationRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumTileService;

public class ItemPlacementCondition : IItemPlacementCondition
{
    private readonly IExhibitRepository _exhibitRepository;
    private readonly IDecorationShopRepository _decorationShopRepository;
    private readonly ISanitationRepository _sanitationRepository;
    private readonly IDecorationOtherRepository _decorationOtherRepository;
    private readonly IMuseumTileRepository _museumTileRepository;

    public ItemPlacementCondition(IExhibitRepository exhibitRepository, IMuseumTileRepository museumTileRepository, IDecorationShopRepository decorationShopRepository, IDecorationOtherRepository decorationOtherRepository, ISanitationRepository sanitationRepository)
    {
        _exhibitRepository = exhibitRepository;
        _museumTileRepository = museumTileRepository;
        _decorationShopRepository = decorationShopRepository;
        _decorationOtherRepository = decorationOtherRepository;
        _sanitationRepository = sanitationRepository;
    }
    
    public async Task<List<ExhibitPlacementConditionData>> CanExhibitBePlacedOnThisTile(string exhibitVariationName)
    {
        var listOfExhibitPlacementConditionData = new List<ExhibitPlacementConditionData>();
        var museumTiles = await _museumTileRepository.GetAll();
        

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

    public async Task<bool> PlaceExhibitOnTile(string tileId, string exhibitVariationName)
    {
        var museumTile = await _museumTileRepository.GetById(tileId);
        if (museumTile != null && museumTile.ItemId != "string") return false;
        if (museumTile == null) return false;
        var exhibit = new Exhibit
        {
            Id = Guid.NewGuid().ToString(),
            ExhibitVariationName = exhibitVariationName,
            XPosition = museumTile.XPosition,
            YPosition = museumTile.YPosition,
            ExhibitDecoration = "string",
            ExhibitArtifactSlot1 = "string",
            ExhibitArtifactSlot2 = "string",
            ExhibitArtifactSlot3 = "string",
            ExhibitArtifactSlot4 = "string",
            ExhibitArtifactSlot5 = "string"
        };
        await _exhibitRepository.Insert(exhibit);
        await _museumTileRepository.UpdateExhibitToMuseumTile(tileId, exhibit.Id);
        return true;
    }
    public async Task<TilesWithExhibitDto> PlaceExhibitOnTiles(string originTileId, List<string> tileIds, string exhibitVariationName, int rotationFrame)
    {
        TilesWithExhibitDto tilesWithExhibitDto = new TilesWithExhibitDto();
        Exhibit exhibit = new Exhibit();
        var museumTiles = await _museumTileRepository.GetAll();
        tilesWithExhibitDto.Exhibit = exhibit;
        tilesWithExhibitDto.MuseumTiles = museumTiles;
        foreach (var tileId in tileIds)
        {
            if (tileId == originTileId)
            {
                var museumTile = await _museumTileRepository.GetById(tileId);
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
                    ExhibitDecoration = "string",
                    ExhibitArtifactSlot1 = "string",
                    ExhibitArtifactSlot2 = "string",
                    ExhibitArtifactSlot3 = "string",
                    ExhibitArtifactSlot4 = "string",
                    ExhibitArtifactSlot5 = "string"
                };
                await _exhibitRepository.Insert(exhibit);
                
            }
        }
        museumTiles = await _museumTileRepository.UpdateExhibitToMuseumTiles(tileIds, exhibit.Id);
        
        tilesWithExhibitDto.Exhibit = exhibit;
        var exhibits = await _exhibitRepository.GetAllExhibits();
        tilesWithExhibitDto.Exhibits = exhibits;
        if (museumTiles != null) tilesWithExhibitDto.MuseumTiles = museumTiles;
        return tilesWithExhibitDto;
    }
    public async Task<TilesWithShopsDTO> PlaceShopOnTiles(string originTileId, List<string> tileIds, string shopVariationName, int rotationFrame)
    {
        TilesWithShopsDTO tilesWithShopsDto = new TilesWithShopsDTO();
        Shop shop = new Shop();
        var museumTiles = await _museumTileRepository.GetAll();
        
        foreach (var tileId in tileIds)
        {
            if (tileId == originTileId)
            {
                var museumTile = await _museumTileRepository.GetById(tileId);
                shop = new Shop
                {
                    Id = Guid.NewGuid().ToString(),
                    ShopVariationName =  shopVariationName,
                    XPosition = museumTile.XPosition,
                    YPosition = museumTile.YPosition,
                    RotationFrame = rotationFrame,
                    
                };
                await _decorationShopRepository.Insert(shop);
                
            }
        }
        museumTiles = await _museumTileRepository.UpdateShopToMuseumTiles(tileIds, shop.Id);
        tilesWithShopsDto.MuseumTiles = museumTiles;
        tilesWithShopsDto.DecorationShops = await _decorationShopRepository.GetAll();
        tilesWithShopsDto.Shop = shop;
        return tilesWithShopsDto;
    }
    public async Task<TilesWithSanitationsDTO> PlaceSanitationOnTiles(string originTileId, List<string> tileIds, string sanitationVariationName, int rotationFrame)
    {
        TilesWithSanitationsDTO tilesWithSanitationsDto = new TilesWithSanitationsDTO();
        Sanitation sanitation = new Sanitation();
        var museumTiles = await _museumTileRepository.GetAll();
        
        foreach (var tileId in tileIds)
        {
            if (tileId == originTileId)
            {
                var museumTile = await _museumTileRepository.GetById(tileId);
                sanitation = new Sanitation()
                {
                    Id = Guid.NewGuid().ToString(),
                    SanitationVariationName =  sanitationVariationName,
                    XPosition = museumTile.XPosition,
                    YPosition = museumTile.YPosition,
                    RotationFrame = rotationFrame,
                    
                };
                await _sanitationRepository.Insert(sanitation);
                
            }
        }
        museumTiles = await _museumTileRepository.UpdateSanitationToMuseumTiles(tileIds, sanitation.Id);
        tilesWithSanitationsDto.MuseumTiles = museumTiles;
        tilesWithSanitationsDto.Sanitations = await _sanitationRepository.GetAllSanitation();
        tilesWithSanitationsDto.Sanitation = sanitation;
        return tilesWithSanitationsDto;
    }
    public async Task<List<MuseumTile>> PlaceOtherDecorationOnTiles(string originTileId, List<string> tileIds, string otherVariationName, int rotationFrame)
    {
        
        DecorationOther decorationOther = new DecorationOther();
        var museumTiles = await _museumTileRepository.GetAll();
        
        foreach (var tileId in tileIds)
        {
            if (tileId == originTileId)
            {
                var museumTile = await _museumTileRepository.GetById(tileId);
                decorationOther = new DecorationOther
                {
                    Id = Guid.NewGuid().ToString(),
                    VariationName =  otherVariationName,
                    XPosition = museumTile.XPosition,
                    YPosition = museumTile.YPosition,
                    RotationFrame = rotationFrame,
                    
                };
                await _decorationOtherRepository.Insert(decorationOther);
                
            }
        }
        museumTiles = await _museumTileRepository.UpdateOtherDecorationToMuseumTiles(tileIds, decorationOther.Id);
        return museumTiles;
    }
}