using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.BuilderCardsRepository;

public interface IBuilderCardRepository
{
    Task<List<TileVariation>?> GetAllTileVariations();
    Task<List<WallVariation>?> GetAllWallVariations();
    Task<List<WallpaperVariation>?> GetAllWallpaperVariations();
}