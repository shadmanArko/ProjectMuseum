using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Models;
using ProjectMuseum.Services.MineService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MineController : ControllerBase
{
    private readonly IMineService _mineService;

    public MineController(IMineService mineService)
    {
        _mineService = mineService;
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
}