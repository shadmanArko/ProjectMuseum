using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Services.MuseumTileService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MuseumTileController : ControllerBase
{
    private readonly IMuseumTileService _museumTileService;

    public MuseumTileController(IMuseumTileService museumTileService)
    {
        _museumTileService = museumTileService;
    }
    
    [HttpGet("GetAllMuseumTiles")]
    public async Task<IActionResult> GetAllMuseumTiles()
    {
        var museumTiles =await _museumTileService.GetAllMuseumTiles();
        return Ok(museumTiles);
    }
    
    [HttpGet("GetAllMuseumTilesForNewGame")]
    public async Task<IActionResult> GetAllMuseumTilesForNewGame()
    {
        var museumTiles =await _museumTileService.GenerateMuseumTileForNewGame();
        return Ok(museumTiles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAllMuseumTileById(string id)
    {
        var museumTile = await _museumTileService.GetMuseumTileById(id);
        return Ok(museumTile);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMuseumTile([FromBody] MuseumTile museumTile)
    {
        var newMuseumTile = await _museumTileService.InsertMuseumTile(museumTile);
        return Ok(newMuseumTile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMuseumTile(string id, [FromBody]MuseumTile museumTile)
    {
        var updatedMuseumTile = await _museumTileService.UpdateMuseumTileById(id, museumTile);
        return Ok(updatedMuseumTile);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMuseumTile(string id)
    {
        var museumTile = await _museumTileService.DeleteMuseumTileById(id);
        return Ok(museumTile);
    }
}