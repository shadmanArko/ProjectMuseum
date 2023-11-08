using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumTileService;

public class MuseumTileDataGenerator : IMuseumTileDataGenerator
{
    private readonly IMuseumTileRepository _museumTileRepository;
    public int numberOfTilesInX = 20;
    public int numberOfTilesInY = 24;
    public int originStartsX = 55;
    public int originStartsY = 22;

    public MuseumTileDataGenerator(IMuseumTileRepository museumTileRepository)
    {
        _museumTileRepository = museumTileRepository;
    }
    
    public async Task<List<MuseumTile>?> GenerateMuseumTileDataForNewMuseum()
    {
        Random r = new Random();
        int probabilityOfDamagedTile = 5; 
        for (int x = originStartsX; x > originStartsX - numberOfTilesInX; x--)
        {
            for (int y = originStartsY; y > originStartsY - numberOfTilesInY; y--)
            {
                var tileSetId = 8;
                if (r.Next(0, 100) <= probabilityOfDamagedTile)
                {
                    tileSetId = r.Next(9, 13);
                }
                var museumTile = new MuseumTile
                {
                    Id = Guid.NewGuid().ToString(),
                    XPosition = x,
                    YPosition = y,
                    TileSetNumber = tileSetId,
                    TileAtlasCoOrdinateX = 0,
                    TileAtlasCoOrdinateY = 0,
                    Layer = 0,
                    WallId = "string",
                    ExhibitId = "string",
                    HangingLightId = "string"
                };
                await _museumTileRepository.Insert(museumTile);
            }
        }
        var museumTiles = await _museumTileRepository.GetAll();
        return museumTiles;
        
    }
}