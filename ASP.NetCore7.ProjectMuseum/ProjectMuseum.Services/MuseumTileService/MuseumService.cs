using AutoMapper;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MuseumRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumTileService;

public class MuseumService : IMuseumService
{
    private readonly IMuseumTileRepository _museumTileRepository;
    private  MuseumTileDataGenerator _museumTileDataGenerator;
    private readonly ExhibitPlacementCondition _exhibitPlacementCondition;
    private readonly IExhibitRepository _exhibitRepository;
    private readonly IMuseumRepository _museumRepository;
    private readonly SaveDataJsonFileDatabase _saveDataJsonFileDatabase;

    public MuseumService(IMuseumTileRepository museumTileRepository, IExhibitRepository exhibitRepository, SaveDataJsonFileDatabase saveDataJsonFileDatabase, IMuseumRepository museumRepository)
    {
        _museumTileRepository = museumTileRepository;
        _exhibitRepository = exhibitRepository;
        _saveDataJsonFileDatabase = saveDataJsonFileDatabase;
        _museumRepository = museumRepository;
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

    public async Task<MuseumTile?> DeleteMuseumTileById(string tileId)
    {
        var museumTile = await _museumTileRepository.Delete(tileId);
        return museumTile;
    }

    public async Task<List<MuseumTile>?> GenerateMuseumTileForNewGame()
    {
       return await _museumTileDataGenerator.GenerateMuseumTileDataForNewMuseum();
    }

    public async Task<int> GetMuseumCurrentMoneyAmount(string id)
    {
        return await _museumRepository.GetMuseumBalance(id);
    }
}