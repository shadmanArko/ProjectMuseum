using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.BuilderCardsRepository;

public class BuilderCardRepository : IBuilderCardRepository
{
    private readonly JsonFileDatabase<TileVariation> _tileVariationDatabase;
    private readonly JsonFileDatabase<WallVariation> _wallVariationDatabase;

    public BuilderCardRepository(JsonFileDatabase<TileVariation> tileVariationDatabase, JsonFileDatabase<WallVariation> wallVariationDatabase)
    {
        _tileVariationDatabase = tileVariationDatabase;
        _wallVariationDatabase = wallVariationDatabase;
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
}