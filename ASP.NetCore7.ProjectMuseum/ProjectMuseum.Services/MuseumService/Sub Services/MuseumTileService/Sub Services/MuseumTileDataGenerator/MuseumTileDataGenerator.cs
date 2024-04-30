using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumTileService;

public class MuseumTileDataGenerator : IMuseumTileDataGenerator
{
    private readonly IMuseumTileRepository _museumTileRepository;
    public int numberOfTilesInX = 18;
    public int numberOfTilesInY = 20;
    public int originStartsX = 0;
    public int originStartsY = 0;

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
                    Walkable = true,
                    ExhibitId = "string",
                    HangingLightId = "string"
                };
                if ((x== 0 || y==0 ) && !((x ==0 && y== -7) || (x ==0 && y== -8)))
                {
                    museumTile.Walkable = false;
                }
                await _museumTileRepository.Insert(museumTile);
            }
        }
        var museumTiles = await _museumTileRepository.GetAll();
        return museumTiles;
        
    }
    public async Task<List<MuseumTile>?> ExpandMuseumTiles(int originPositionX, int originPositionY)
    {
        Random r = new Random();
        int probabilityOfDamagedTile = 5; 
        for (int x = originPositionX; x > originPositionX - numberOfTilesInX; x--)
        {
            for (int y = originPositionY; y > originPositionY - numberOfTilesInY; y--)
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
                    Walkable = true,
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