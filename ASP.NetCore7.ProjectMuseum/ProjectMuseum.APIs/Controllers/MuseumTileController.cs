using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Services.LoadAndSaveService;
using ProjectMuseum.Services.MuseumService;
using ProjectMuseum.Services.MuseumTileService;
using ProjectMuseum.Services.PlayerInfoService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MuseumTileController : ControllerBase
{
    private readonly IMuseumTileService _museumTileService;
    private readonly IMuseumService _museumService;
    private readonly ISaveService _saveService;
    private readonly ILoadService _loadService;
    private readonly IPlayerInfoService _playerInfoService;
    public MuseumTileController(IMuseumTileService museumTileService, IMuseumService museumService, ILoadService loadService, ISaveService saveService, IPlayerInfoService playerInfoService)
    {
        _museumTileService = museumTileService;
        _museumService = museumService;
        _loadService = loadService;
        _saveService = saveService;
        _playerInfoService = playerInfoService;
    }
    
    [HttpGet("GetAllMuseumTiles")]
    public async Task<IActionResult> GetAllMuseumTiles()
    {
        var museumTiles =await _museumTileService.GetAllMuseumTiles();
        return Ok(museumTiles);
    }
    [HttpGet("PlaceAnExhibit/{tileId}/{exhibitType}")]
    public async Task<IActionResult> PlaceAnExhibit(string tileId, string exhibitType )
    {
        var exhibitPlacementResult = await _museumTileService.PlaceExhibitOnTile(tileId, exhibitType);
        return Ok(exhibitPlacementResult);
    }
    
    [HttpGet("GetAllMuseumTilesForNewGame")]
    public async Task<IActionResult> GetAllMuseumTilesForNewGame()
    {
        var museumTiles =await _museumTileService.GenerateMuseumTileForNewGame();
        return Ok(museumTiles);
    }
    [HttpGet("SaveData")]
    public async Task<IActionResult> SaveData()
    {
        await _saveService.SaveData();
        return Ok();
    }
    [HttpGet("LoadData")]
    public async Task<IActionResult> LoadData()
    {
        await _loadService.LoadData();
        return Ok();
    }
    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetAllMuseumTileById(string id)
    // {
    //     var museumTile = await _museumTileService.GetMuseumTileById(id);
    //     return Ok(museumTile);
    // }

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
    [HttpGet("{exhibitType}")]
    public async Task<IActionResult> GetEligibilityOfPositioningExhibit(string exhibitType)
    {
        var exhibitEligibility = await _museumTileService.GetEligibilityOfPositioningExhibit( exhibitType);
        return Ok(exhibitEligibility);
    }
    [HttpGet("GetMuseumBalance/{id}")]
    public async Task<IActionResult> GetMuseumBalance(string id)
    {
        var exhibitEligibility = await _museumService.GetMuseumCurrentMoneyAmount(id);
        return Ok(exhibitEligibility);
    }
    [HttpGet("AddToMuseumBalance/{id}/{amount}")]
    public async Task<IActionResult> AddToMuseumBalance(string id, float amount)
    {
        var addToMuseumBalance = await _museumService.AddToMuseumBalance(id, amount);
        return Ok(addToMuseumBalance);
    }
    [HttpGet("ReduceMuseumBalance/{id}/{amount}")]
    public async Task<IActionResult> ReduceMuseumBalance(string id, float amount)
    {
        var reduceMuseumBalance = await _museumService.ReduceMuseumBalance(id, amount);
        return Ok(reduceMuseumBalance);
    }
    [HttpPost("PostPlayerInfo")]
    public async Task<IActionResult> CreatePlayerInfo([FromBody] PlayerInfo playerInfo)
    {
        var newPlayerInfo = await _playerInfoService.InsertPlayerInfo(playerInfo);
        return Ok(newPlayerInfo);
    }
}