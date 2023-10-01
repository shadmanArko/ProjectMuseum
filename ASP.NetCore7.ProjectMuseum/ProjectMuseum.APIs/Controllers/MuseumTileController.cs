using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Services;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[ApiController]
public class MuseumTileController : ControllerBase
{
    private readonly JsonFileService<MuseumTile> _museumTileService;
    private readonly IMapper _mapper;

    public MuseumTileController(JsonFileService<MuseumTile> museumTileService, IMapper mapper)
    {
        _museumTileService = museumTileService;
        _mapper = mapper;
    }
    
    [Route("api/museumtiles")]
    [HttpGet]
    public async Task<IActionResult> GetAllMuseumTiles()
    {
        var museumTiles = await _museumTileService.ReadDataAsync();
        var museumTileDtos = _mapper.Map<List<MuseumTileDto>>(museumTiles);
        return Ok(museumTileDtos);
    }
}