using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Models;
using ProjectMuseum.Services.MineService;
using ProjectMuseum.Services.MineService.Sub_Services;
using ProjectMuseum.Services.MineService.Sub_Services.MineCellService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MineController : ControllerBase
{
    private readonly IMineService _mineService;
    private readonly IMineArtifactService _mineArtifactService;
    private readonly IMineCellGenerator _mineCellGenerator;

    public MineController(IMineService mineService, IMineArtifactService mineArtifactService, IMineCellGenerator mineCellGenerator)
    {
        _mineService = mineService;
        _mineArtifactService = mineArtifactService;
        _mineCellGenerator = mineCellGenerator;
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

    [HttpGet("GenerateMine")]
    public async Task<IActionResult> GenerateMine()
    {
        var mine = await _mineCellGenerator.GenerateMineCellData();
        return Ok(mine);
    }


}