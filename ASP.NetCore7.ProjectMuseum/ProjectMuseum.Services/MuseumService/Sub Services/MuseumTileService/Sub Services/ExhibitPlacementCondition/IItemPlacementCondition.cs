using System.Globalization;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumTileService;

public interface IItemPlacementCondition
{
     Task<List<ExhibitPlacementConditionData>> CanExhibitBePlacedOnThisTile(string exhibitVariationName);
     Task<bool> PlaceExhibitOnTile(string tileId, string exhibitVariationName);
     Task<TilesWithExhibitDto> PlaceExhibitOnTiles(string originTileId, List<string> tileIds,
          string exhibitVariationName, int rotationFrame);
     Task<TilesWithShopsDTO> PlaceShopOnTiles(string originTileId, List<string> tileIds,
         string shopVariationName, int rotationFrame);
}