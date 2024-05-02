using AutoMapper;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.DecorationOtherRepository;
using ProjectMuseum.Repositories.DecorationRepository;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MuseumRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.SanitationRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumTileService;

public class MuseumTileService : IMuseumTileService
{
    private readonly IMuseumTileRepository _museumTileRepository;
    private  MuseumTileDataGenerator _museumTileDataGenerator;
    private readonly ItemPlacementCondition _itemPlacementCondition;
    private readonly IExhibitRepository _exhibitRepository;
    private readonly IDecorationShopRepository _decorationShopRepository;
    private readonly IDecorationOtherRepository _decorationOtherRepository;
    private readonly ISanitationRepository _sanitationRepository;
    
    private readonly SaveDataJsonFileDatabase _saveDataJsonFileDatabase;

    public MuseumTileService(IMuseumTileRepository museumTileRepository, IExhibitRepository exhibitRepository, SaveDataJsonFileDatabase saveDataJsonFileDatabase, IDecorationShopRepository decorationShopRepository, IDecorationOtherRepository decorationOtherRepository, ISanitationRepository sanitationRepository)
    {
        _museumTileRepository = museumTileRepository;
        _exhibitRepository = exhibitRepository;
        _saveDataJsonFileDatabase = saveDataJsonFileDatabase;
        _decorationShopRepository = decorationShopRepository;
        _decorationOtherRepository = decorationOtherRepository;
        _sanitationRepository = sanitationRepository;
        _museumTileDataGenerator = new MuseumTileDataGenerator(_museumTileRepository);
        _itemPlacementCondition = new ItemPlacementCondition(_exhibitRepository, _museumTileRepository, _decorationShopRepository, _decorationOtherRepository, _sanitationRepository);
    }

    public async Task<MuseumTile> InsertMuseumTile(MuseumTile museumTile)
    {
        var newMuseumTile = museumTile;
        newMuseumTile.Id = Guid.NewGuid().ToString();
        await _museumTileRepository.Insert(museumTile);
        return newMuseumTile;
    }

    public async Task<MuseumTile?> GetMuseumTileById(string tileId)
    {
        var museumTile = await _museumTileRepository.GetById(tileId);
        return museumTile;
    }

    public async Task<List<MuseumTile>?> GetAllMuseumTiles()
    {
        var museumTiles = await _museumTileRepository.GetAll();
        return museumTiles;
    }

    public Task<List<Exhibit>?> GetAllExhibits()
    {
        throw new NotImplementedException();
    }

    public async Task<List<ExhibitPlacementConditionData>> GetEligibilityOfPositioningExhibit(string exhibitVariationName)
    {
        return await _itemPlacementCondition.CanExhibitBePlacedOnThisTile(exhibitVariationName);
    }

    public async Task<bool> PlaceExhibitOnTile(string tileId, string exhibitVariationName)
    {
        return await _itemPlacementCondition.PlaceExhibitOnTile(tileId, exhibitVariationName);
    }

    public async Task<TilesWithExhibitDto> PlaceExhibitOnTiles(string originTileId, List<string> tileIds, string exhibitVariationName, int rotationFrame)
    {
        return await _itemPlacementCondition.PlaceExhibitOnTiles(originTileId, tileIds, exhibitVariationName, rotationFrame);
    }

    public async Task<TilesWithShopsDTO> PlaceShopOnTiles(string originTileId, List<string> tileIds, string shopVariationName, int rotationFrame)
    {
        return await _itemPlacementCondition.PlaceShopOnTiles(originTileId, tileIds, shopVariationName, rotationFrame);
    }

    public async Task<TilesWithSanitationsDTO> PlaceSanitationOnTiles(string originTileId, List<string> tileIds, string sanitationVariationName, int rotationFrame)
    {
        return await _itemPlacementCondition.PlaceSanitationOnTiles(originTileId, tileIds, sanitationVariationName, rotationFrame);
    }
    public async Task<List<MuseumTile>> PlaceOtherDecorationOnTiles(string originTileId, List<string> tileIds, string otherVariationName, int rotationFrame)
    {
        return await _itemPlacementCondition.PlaceOtherDecorationOnTiles(originTileId, tileIds, otherVariationName, rotationFrame);

    }
    
    public async Task<MuseumTile> UpdateMuseumTileById(string tileId, MuseumTile museumTile)
    {
        var updatedMuseumTile = await _museumTileRepository.Update(tileId, museumTile);
        return updatedMuseumTile;
    }

    public async Task<List<MuseumTile>?> UpdateMuseumTilesSourceId(List<string> museumTilesId, int sourceId)
    {
        var updatedMuseumTile = await _museumTileRepository.UpdateMuseumTilesSourceId(museumTilesId, sourceId);
        return updatedMuseumTile;
    }

    public async Task<List<MuseumTile>?> UpdateMuseumTilesWallId(List<TileWallInfo> tileWallInfos)
    {
        var updatedMuseumTile = await _museumTileRepository.UpdateMuseumTilesWallId(tileWallInfos);
        return updatedMuseumTile;
    }

    public async Task<MuseumTile?> DeleteMuseumTileById(string tileId)
    {
        var museumTile = await _museumTileRepository.Delete(tileId);
        return museumTile;
    }

    public async Task<List<MuseumTile>?> DeleteAllMuseumTiles()
    {
        var museumTiles = await _museumTileRepository.DeleteAll();
        return museumTiles;
    }

    public async Task<List<MuseumTile>?> GenerateMuseumTileForNewGame()
    {
       return await _museumTileDataGenerator.GenerateMuseumTileDataForNewMuseum();
    }
    public async Task<List<MuseumTile>?> ExpandMuseumTiles(int originPositionX, int originPositionY)
    {
        return await _museumTileDataGenerator.ExpandMuseumTiles(originPositionX, originPositionY);
    }
    
}