using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IExhibitPlacementCondition
{
     Task<bool> CanExhibitBePlacedOnThisTile(string exhibitType, int tileXPosition, int tileYPosition);
}