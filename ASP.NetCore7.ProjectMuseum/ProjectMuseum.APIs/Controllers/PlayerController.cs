using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Models;
using ProjectMuseum.Services.InventorySevice;
using ProjectMuseum.Services.LoadAndSaveService;
using ProjectMuseum.Services.PlayerInfoService;
using ProjectMuseum.Services.PlayerService.Sub_Services.TimeService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;


[Route("api/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
    private readonly ISaveService _saveService;
    private readonly ILoadService _loadService;
    private readonly IPlayerInfoService _playerInfoService;
    private readonly IInventoryService _inventoryService;
    private readonly ITimeService _timeService;

    public PlayerController(ISaveService saveService, ILoadService loadService, IPlayerInfoService playerInfoService, IInventoryService inventoryService, ITimeService timeService)
    {
        _saveService = saveService;
        _loadService = loadService;
        _playerInfoService = playerInfoService;
        _inventoryService = inventoryService;
        _timeService = timeService;
    }
    [HttpPost("PostPlayerInfo")]
    public async Task<IActionResult> CreatePlayerInfo([FromBody] PlayerInfo playerInfo)
    {
        var newPlayerInfo = await _playerInfoService.InsertPlayerInfo(playerInfo);
        return Ok(newPlayerInfo);
    }
    [HttpGet("GetPlayerInfo")]
    public async Task<IActionResult> GetPlayerInfo()
    {
        var newPlayerInfo = await _playerInfoService.GetPlayerInfo();
        return Ok(newPlayerInfo);
    }
    [HttpGet("UpdateCompletedTutorial/{completedTutorialNumber}")]
    public async Task<IActionResult> UpdateCompletedTutorial(int completedTutorialNumber)
    {
        var newPlayerInfo = await _playerInfoService.UpdateCompletedTutorial(completedTutorialNumber);
        return Ok(newPlayerInfo);
    }
    [HttpGet("UpdateCompletedStory/{completedStoryNumber}")]
    public async Task<IActionResult> UpdateCompletedStory(int completedStoryNumber)
    {
        var newPlayerInfo = await _playerInfoService.UpdateCompletedStory(completedStoryNumber);
        return Ok(newPlayerInfo);
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
    [HttpGet("LoadDataForNewGame")]
    public async Task<IActionResult> LoadDataForNewGame()
    {
        await _loadService.LoadDataForNewGame();
        return Ok();
    }

    [HttpGet("GetAllWeaponsInInventory")]
    public async Task<IActionResult> GetAllWeaponsInInventory()
    {
        var weapons = await _inventoryService.GetAllEquipables();
        return Ok(weapons);
    }
    
    [HttpGet("GetAllArtifactsInInventory")]
    public async Task<IActionResult> GetAllArtifactsInInventory()
    {
        var artifacts = await _inventoryService.GetAllArtifacts();
        return Ok(artifacts);
    }

    [HttpGet("SendAllArtifactsFromInventoryToArtifactStorage")]
    public async Task<IActionResult> SendAllArtifactsFromInventoryToArtifactStorage()
    {
        await _inventoryService.SendAllArtifactsToArtifactStorage();
        return Ok();
    }

    [HttpGet("GetTime")]
    public async Task<IActionResult> GetTime()
    {
        var time = await _timeService.GetTime();
        return Ok(time);
    }

    [HttpPost("SaveTime")]
    public async Task<IActionResult> SaveTime([FromBody]Time time)
    {
        var t = await _timeService.SaveTime(time);
        return Ok(t);
    }

    [HttpGet("GetInventory")]
    public async Task<IActionResult> GetInventory()
    {
        var inventory = await _inventoryService.GetInventory();
        return Ok(inventory);
    }

    // [HttpGet("SendItemFromInventoryToMine/{inventoryItemId}")]
    // public async Task<IActionResult> SendItemFromInventoryToMine(string inventoryItemId)
    // {
    //     var inventoryItem = await _inventoryService.SendItemFromInventoryToMine(inventoryItemId);
    //     return Ok(inventoryItem);
    // }
}