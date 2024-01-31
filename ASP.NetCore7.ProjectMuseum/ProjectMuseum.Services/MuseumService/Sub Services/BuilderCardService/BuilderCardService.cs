using ProjectMuseum.Models;
using ProjectMuseum.Repositories.BuilderCardsRepository;

namespace ProjectMuseum.Services.BuilderCardService;

public class BuilderCardService: IBuilderCardService
{
    private readonly IBuilderCardRepository _builderCardRepository;

    public BuilderCardService(IBuilderCardRepository builderCardRepository)
    {
        _builderCardRepository = builderCardRepository;
    }
    public async Task<List<TileVariation>?> GetAllTileVariations()
    {
        var tileVariations =  await _builderCardRepository.GetAllTileVariations();
        return tileVariations;
    }

    public async Task<List<WallVariation>?> GetAllWallVariations()
    {
        var wallVariations =  await _builderCardRepository.GetAllWallVariations();
        return wallVariations;
        
    }

    public async Task<List<WallpaperVariation>?> GetAllWallpaperVariations()
    {
        var variations =  await _builderCardRepository.GetAllWallpaperVariations();
        return variations;
    }
}