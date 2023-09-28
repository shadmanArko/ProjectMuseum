using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Models;
using ProjectMuseum.Services;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[ApiController]
public class MuseumTileController : ControllerBase
{
    private readonly JsonFileService<MuseumTile> _museumTileService;

    public MuseumTileController(JsonFileService<MuseumTile> museumTileService)
    {
        _museumTileService = museumTileService;
    }
    
    [Route("api/museumtiles")]
    [HttpGet]
    public async Task<IActionResult> GetAllMuseumTiles()
    {
        var museumTiles = await _museumTileService.ReadDataAsync();
        return Ok(museumTiles);
    }
}