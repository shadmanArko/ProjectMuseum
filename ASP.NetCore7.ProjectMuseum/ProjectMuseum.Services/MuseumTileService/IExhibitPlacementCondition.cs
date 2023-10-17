using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IExhibitPlacementCondition
{
     Task<List<ExhibitPlacementConditionData>> CanExhibitBePlacedOnThisTile(string exhibitType);
}