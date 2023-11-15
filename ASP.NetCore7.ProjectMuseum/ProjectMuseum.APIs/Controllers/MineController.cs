using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Models;
using ProjectMuseum.Services.MineService;
using ProjectMuseum.Services.MineService.Sub_Services;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MineController : ControllerBase
{
    private readonly IMineService _mineService;
    private readonly IMineArtifactService _mineArtifactService;

    public MineController(IMineService mineService, IMineArtifactService mineArtifactService)
    {
        _mineService = mineService;
        _mineArtifactService = mineArtifactService;
    }

    [HttpGet("GetMineData")]
    public async Task<IActionResult> GetMineData()
    {
        var mine = await _mineService.GetMineData();
        return Ok(mine);
    }
    
    [HttpPut("UpdateMineData")]
    public async Task<IActionResult> UpdateMineData([FromBody] Mine mine)
    {
        var newMine = await _mineService.UpdateMine(mine);
        return Ok(newMine);
    }
    
    [HttpPut("SendArtifactToInventory")]
    public async Task<IActionResult> SendArtifactFromMineToInventory(string id)
    {
        var artifact = await _mineArtifactService.SendArtifactToInventory(id);
        return Ok(artifact);
    }
}