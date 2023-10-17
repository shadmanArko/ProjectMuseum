using System.Globalization;
using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IExhibitPlacementCondition
{
     Task<List<ExhibitPlacementConditionData>> CanExhibitBePlacedOnThisTile(string exhibitType);
     Task<bool> PlaceExhibitOnTile(string tileId, string exhibitType);
}