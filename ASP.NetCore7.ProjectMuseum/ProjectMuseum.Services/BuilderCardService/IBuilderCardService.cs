using ProjectMuseum.Models;

namespace ProjectMuseum.Services.BuilderCardService;

public interface IBuilderCardService
{
    Task<List<TileVariation>?> GetAllTileVariations();
    Task<List<WallVariation>?> GetAllWallVariations();
}