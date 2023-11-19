using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Models;
using ProjectMuseum.Services.MuseumService;
using ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactStorageService;
using ProjectMuseum.Services.MuseumService.Sub_Services.DisplayArtifactService;
using ProjectMuseum.Services.MuseumService.Sub_Services.TradingArtifactsService;
using ProjectMuseum.Services.MuseumTileService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;


[Route("api/[controller]")]
[ApiController]
public class MuseumController : ControllerBase
{ 
    private readonly IMuseumTileService _museumTileService;
    private readonly IMuseumService _museumService;
    private readonly IDisplayArtifactService _displayArtifactService;
    private readonly IArtifactStorageService _artifactStorageService;
    private readonly ITradingArtifactsService _tradingArtifactsService;

    public MuseumController(IMuseumTileService museumTileService, IMuseumService museumService, IDisplayArtifactService displayArtifactService, IArtifactStorageService artifactStorageService, ITradingArtifactsService tradingArtifactsService)
    {
        _museumTileService = museumTileService;
        _museumService = museumService;
        _displayArtifactService = displayArtifactService;
        _artifactStorageService = artifactStorageService;
        _tradingArtifactsService = tradingArtifactsService;
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
    [HttpPost]
    public async Task<IActionResult> CreateMuseumTile([FromBody] MuseumTile museumTile)
    {
        var newMuseumTile = await _museumTileService.InsertMuseumTile(museumTile);
        return Ok(newMuseumTile);
    }
    //[HttpGet(ArtifactName = "GetWeatherForecast")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMuseumTile(string id, [FromBody]MuseumTile museumTile)
    {
        var updatedMuseumTile = await _museumTileService.UpdateMuseumTileById(id, museumTile);
        return Ok(updatedMuseumTile);
    }
    [HttpPut("UpdateMuseumTilesSourceId")]
    public async Task<IActionResult> UpdateMuseumTilesSourceId(List<string> ids, int sourceId)
    {
        var updatedMuseumTile = await _museumTileService.UpdateMuseumTilesSourceId(ids, sourceId);
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

    [HttpGet("GetAllDisplayArtifacts")]
    public async Task<IActionResult> GetAllDisplayArtifacts()
    {
        var artifacts = await _displayArtifactService.GetAllArtifacts();
        return Ok(artifacts);
    }

    [HttpGet("GetAllArtifactsInStorage")]
    public async Task<IActionResult> GetAllArtifactsInStorage()
    {
        var artifacts = await _artifactStorageService.GetAllArtifactsOfStorage();
        return Ok(artifacts);
    }

    [HttpGet("GetAllTradingArtifacts")]
    public async Task<IActionResult> GetAllTradingArtifacts()
    {
        var artifacts = await _tradingArtifactsService.GetAllArtifacts();
        return Ok(artifacts);
    }

    [HttpPost("AddArtifactToTrading")]
    public async Task<IActionResult> AddArtifactToTrading([FromBody]Artifact newArtifact)
    {
        var artifact = await _tradingArtifactsService.AddArtifact(newArtifact);
        return Ok(artifact);
    }

}