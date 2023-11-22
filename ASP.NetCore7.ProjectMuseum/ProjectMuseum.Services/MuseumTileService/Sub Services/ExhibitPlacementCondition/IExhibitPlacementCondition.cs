using System.Globalization;
using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IExhibitPlacementCondition
{
     Task<List<ExhibitPlacementConditionData>> CanExhibitBePlacedOnThisTile(string exhibitVariationName);
     Task<bool> PlaceExhibitOnTile(string tileId, string exhibitVariationName);
}