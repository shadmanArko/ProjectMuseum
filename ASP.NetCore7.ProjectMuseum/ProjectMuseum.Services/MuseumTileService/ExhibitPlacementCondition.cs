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
    
    public async Task<List<ExhibitPlacementConditionData>> CanExhibitBePlacedOnThisTile(string exhibitType)
    {
        var listOfExhibitPlacementConditionData = new List<ExhibitPlacementConditionData>();
        var museumTiles = await _museumTileRepository.GetAll();
        switch (exhibitType)
        {
            case "small":

                foreach (var museumTile in museumTiles)
                {
                    var exhibitTilePlacementData = new ExhibitPlacementConditionData();
                    exhibitTilePlacementData.Id = museumTile.Id;
                    exhibitTilePlacementData.TileXPosition = museumTile.XPosition;
                    exhibitTilePlacementData.TileYPosition = museumTile.YPosition;
                    exhibitTilePlacementData.IsEligible = museumTile.ExhibitId == "string";
                    listOfExhibitPlacementConditionData.Add(exhibitTilePlacementData);
                }
                break;
        }

        return listOfExhibitPlacementConditionData;
    }
}