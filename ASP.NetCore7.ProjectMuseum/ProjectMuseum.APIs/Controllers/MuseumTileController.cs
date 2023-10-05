using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumTileRepository;
using ProjectMuseum.Services;
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

    //[Route("api/museumtiles")]
    [HttpGet]
    public async Task<IActionResult> GetAllMuseumTiles()
    {
        var museumTileDtos =await _museumTileService.GetAllMuseumTiles();
        return Ok(museumTileDtos);
    }

    // [HttpPost]
    // public async Task<IActionResult> CreateMuseumTile(MuseumTileDto museumTileDto)
    // {
    //     var museumTile = new MuseumTile();
    //     museumTile.Id = Guid.NewGuid().ToString();
    //     museumTile.XPosition = museumTileDto.XPosition;
    //     museumTile.YPosition = museumTileDto.YPosition;
    //     museumTile.Decoration = museumTileDto.Decoration;
    //     museumTile.Flooring = museumTile.Flooring;
    //     var museumTiles = await _museumTileService.ReadDataAsync();
    //     museumTiles?.Add(museumTile);
    //     if (museumTiles != null) await _museumTileService.WriteDataAsync(museumTiles);
    //     return Ok(museumTile);
    // }
}