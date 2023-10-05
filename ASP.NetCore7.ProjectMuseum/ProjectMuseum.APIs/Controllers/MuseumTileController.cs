using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.DTOs;
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
    
    [HttpGet]
    public async Task<IActionResult> GetAllMuseumTiles()
    {
        var museumTileDtos =await _museumTileService.GetAllMuseumTiles();
        return Ok(museumTileDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAllMuseumTileById(string id)
    {
        var museumTileDto = await _museumTileService.GetMuseumTileById(id);
        return Ok(museumTileDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMuseumTile([FromForm] MuseumTileDto museumTileDto)
    {
        var newMuseumTileDto = await _museumTileService.InsertMuseumTile(museumTileDto);
        return Ok(newMuseumTileDto);
    }
}