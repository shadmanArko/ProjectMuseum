using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.BuilderCardsRepository;

public class BuilderCardRepository : IBuilderCardRepository
{
    private readonly JsonFileDatabase<TileVariation> _tileVariationDatabase;
    private readonly JsonFileDatabase<WallVariation> _wallVariationDatabase;
    private readonly JsonFileDatabase<WallpaperVariation> _wallpaperVariationDatabase;

    public BuilderCardRepository(JsonFileDatabase<TileVariation> tileVariationDatabase, JsonFileDatabase<WallVariation> wallVariationDatabase, JsonFileDatabase<WallpaperVariation> wallpaperVariationDatabase)
    {
        _tileVariationDatabase = tileVariationDatabase;
        _wallVariationDatabase = wallVariationDatabase;
        _wallpaperVariationDatabase = wallpaperVariationDatabase;
    }
    public async Task<List<TileVariation>?> GetAllTileVariations()
    {
        var tileVariations = await _tileVariationDatabase.ReadDataAsync();
        return tileVariations;
    }

    public async Task<List<WallVariation>?> GetAllWallVariations()
    {
        var wallVariations = await _wallVariationDatabase.ReadDataAsync();
        return wallVariations;
    }
    public async Task<List<WallpaperVariation>?> GetAllWallpaperVariations()
    {
        var wallpaperVariations = await _wallpaperVariationDatabase.ReadDataAsync();
        return wallpaperVariations;
    }
}