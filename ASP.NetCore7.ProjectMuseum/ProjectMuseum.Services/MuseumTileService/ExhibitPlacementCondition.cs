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
    
    public async Task<bool> CanExhibitBePlacedOnThisTile(string exhibitType, int tileXPosition, int tileYPosition)
    {
        switch (exhibitType)
        {
            case "small":
                var museumTile = await _museumTileRepository.GetByPosition(tileXPosition, tileYPosition);
                return (museumTile != null && museumTile.ExhibitId.Equals("string"));
        }

        return false;
    }
}