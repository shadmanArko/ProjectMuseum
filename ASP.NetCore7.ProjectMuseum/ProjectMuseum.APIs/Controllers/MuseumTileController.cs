using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Services.MuseumTileService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MuseumTileController : ControllerBase
{
    private readonly IMuseumService _museumService;

    public MuseumTileController(IMuseumService museumService)
    {
        _museumService = museumService;
    }
    
    [HttpGet("GetAllMuseumTiles")]
    public async Task<IActionResult> GetAllMuseumTiles()
    {
        var museumTiles =await _museumService.GetAllMuseumTiles();
        return Ok(museumTiles);
    }
    
    [HttpGet("GetAllMuseumTilesForNewGame")]
    public async Task<IActionResult> GetAllMuseumTilesForNewGame()
    {
        var museumTiles =await _museumService.GenerateMuseumTileForNewGame();
        return Ok(museumTiles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAllMuseumTileById(string id)
    {
        var museumTile = await _museumService.GetMuseumTileById(id);
        return Ok(museumTile);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMuseumTile([FromBody] MuseumTile museumTile)
    {
        var newMuseumTile = await _museumService.InsertMuseumTile(museumTile);
        return Ok(newMuseumTile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMuseumTile(string id, [FromBody]MuseumTile museumTile)
    {
        var updatedMuseumTile = await _museumService.UpdateMuseumTileById(id, museumTile);
        return Ok(updatedMuseumTile);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMuseumTile(string id)
    {
        var museumTile = await _museumService.DeleteMuseumTileById(id);
        return Ok(museumTile);
    }
    [HttpGet("{exhibitType}/{tileXPosition}/{tileYPosition}")]
    public async Task<IActionResult> GetEligibilityOfPositioningExhibit(string exhibitType, int tileXPosition, int tileYPosition)
    {
        var exhibitEligibility = await _museumService.GetEligibilityOfPositioningExhibit( exhibitType,  tileXPosition,  tileYPosition);
        return Ok(exhibitEligibility);
    }
}