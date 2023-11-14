using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Models;
using ProjectMuseum.Services.InventorySevice;
using ProjectMuseum.Services.LoadAndSaveService;
using ProjectMuseum.Services.PlayerInfoService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;


[Route("api/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
    private readonly ISaveService _saveService;
    private readonly ILoadService _loadService;
    private readonly IPlayerInfoService _playerInfoService;
    private readonly IInventoryService _inventoryService;

    public PlayerController(ISaveService saveService, ILoadService loadService, IPlayerInfoService playerInfoService, IInventoryService inventoryService)
    {
        _saveService = saveService;
        _loadService = loadService;
        _playerInfoService = playerInfoService;
        _inventoryService = inventoryService;
    }
    [HttpPost("PostPlayerInfo")]
    public async Task<IActionResult> CreatePlayerInfo([FromBody] PlayerInfo playerInfo)
    {
        var newPlayerInfo = await _playerInfoService.InsertPlayerInfo(playerInfo);
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

    [HttpGet("GetAllWeaponsInInventory")]
    public async Task<IActionResult> GetAllWeaponsInInventory()
    {
        var weapons = await _inventoryService.GetAllWeapons();
        return Ok(weapons);
    }
    
    [HttpGet("GetAllArtifactsInInventory")]
    public async Task<IActionResult> GetAllArtifactsInInventory()
    {
        var artifacts = await _inventoryService.GetAllArtifacts();
        return Ok(artifacts);
    }
}