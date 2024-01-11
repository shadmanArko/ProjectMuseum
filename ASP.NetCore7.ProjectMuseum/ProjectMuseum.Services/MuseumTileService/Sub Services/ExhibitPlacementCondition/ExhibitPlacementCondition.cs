using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumTileService;

public class ExhibitPlacementCondition : IExhibitPlacementCondition
{
    private readonly IExhibitRepository _exhibitRepository;
    private readonly IMuseumTileRepository _museumTileRepository;

    public ExhibitPlacementCondition(IExhibitRepository exhibitRepository, IMuseumTileRepository museumTileRepository)
    {
        _exhibitRepository = exhibitRepository;
        _museumTileRepository = museumTileRepository;
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
            exhibitTilePlacementData.IsEligible = museumTile.ExhibitId == "string";
            listOfExhibitPlacementConditionData.Add(exhibitTilePlacementData);
        }
        

        return listOfExhibitPlacementConditionData;
    }

    public async Task<bool> PlaceExhibitOnTile(string tileId, string exhibitVariationName)
    {
        var museumTile = await _museumTileRepository.GetById(tileId);
        if (museumTile != null && museumTile.ExhibitId != "string") return false;
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
                if (museumTile != null && museumTile.ExhibitId != "string") return tilesWithExhibitDto;
                if (museumTile == null) return tilesWithExhibitDto;
                exhibit = new Exhibit
                {
                    Id = Guid.NewGuid().ToString(),
                    ExhibitVariationName = exhibitVariationName,
                    XPosition = museumTile.XPosition,
                    YPosition = museumTile.YPosition,
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
        await _museumTileRepository.UpdateExhibitToMuseumTiles(tileIds, exhibit.Id);
        tilesWithExhibitDto.Exhibit = exhibit;
        var exhibits = await _exhibitRepository.GetAllExhibits();
        tilesWithExhibitDto.Exhibits = exhibits;
        return tilesWithExhibitDto;
    }
}