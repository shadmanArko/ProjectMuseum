using AutoMapper;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MuseumRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumTileService;

public class MuseumTileService : IMuseumTileService
{
    private readonly IMuseumTileRepository _museumTileRepository;
    private  MuseumTileDataGenerator _museumTileDataGenerator;
    private readonly ExhibitPlacementCondition _exhibitPlacementCondition;
    private readonly IExhibitRepository _exhibitRepository;
    
    private readonly SaveDataJsonFileDatabase _saveDataJsonFileDatabase;

    public MuseumTileService(IMuseumTileRepository museumTileRepository, IExhibitRepository exhibitRepository, SaveDataJsonFileDatabase saveDataJsonFileDatabase)
    {
        _museumTileRepository = museumTileRepository;
        _exhibitRepository = exhibitRepository;
        _saveDataJsonFileDatabase = saveDataJsonFileDatabase;
        _museumTileDataGenerator = new MuseumTileDataGenerator(_museumTileRepository);
        _exhibitPlacementCondition = new ExhibitPlacementCondition(_exhibitRepository, _museumTileRepository);
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

    public async Task<List<ExhibitPlacementConditionData>> GetEligibilityOfPositioningExhibit(string exhibitType)
    {
        return await _exhibitPlacementCondition.CanExhibitBePlacedOnThisTile(exhibitType);
    }

    public async Task<bool> PlaceExhibitOnTile(string tileId, string exhibitType)
    {
        return await _exhibitPlacementCondition.PlaceExhibitOnTile(tileId, exhibitType);
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

    public async Task<MuseumTile?> DeleteMuseumTileById(string tileId)
    {
        var museumTile = await _museumTileRepository.Delete(tileId);
        return museumTile;
    }

    public async Task<List<MuseumTile>?> GenerateMuseumTileForNewGame()
    {
       return await _museumTileDataGenerator.GenerateMuseumTileDataForNewMuseum();
    }

    
}