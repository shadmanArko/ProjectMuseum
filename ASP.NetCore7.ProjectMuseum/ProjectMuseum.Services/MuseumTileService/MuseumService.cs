using AutoMapper;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumTileService;

public class MuseumService : IMuseumService
{
    private readonly IMuseumTileRepository _museumTileRepository;
    private  MuseumTileDataGenerator _museumTileDataGenerator;
    private readonly ExhibitPlacementCondition _exhibitPlacementCondition;
    private readonly IExhibitRepository _exhibitRepository;

    public MuseumService(IMuseumTileRepository museumTileRepository, IExhibitRepository exhibitRepository)
    {
        _museumTileRepository = museumTileRepository;
        _exhibitRepository = exhibitRepository;
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

    public async Task<bool> GetEligibilityOfPositioningExhibit(string exhibitType, int tileXPosition, int tileYPosition)
    {
        return await _exhibitPlacementCondition.CanExhibitBePlacedOnThisTile(exhibitType, tileXPosition, tileYPosition);
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
}