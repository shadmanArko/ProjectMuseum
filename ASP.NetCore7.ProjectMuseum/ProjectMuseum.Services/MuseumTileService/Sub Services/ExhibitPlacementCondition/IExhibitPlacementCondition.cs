using System.Globalization;
using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IExhibitPlacementCondition
{
     Task<List<ExhibitPlacementConditionData>> CanExhibitBePlacedOnThisTile(string exhibitVariationName);
     Task<bool> PlaceExhibitOnTile(string tileId, string exhibitVariationName);
     Task<bool> PlaceExhibitOnTiles(string originTileId, List<string> tileIds, string exhibitVariationName);
}