using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Services.MiscellaneousDataService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MiscellaneousDataController: ControllerBase
{
    private readonly IMiscellaneousDataService _miscellaneousDataService;

    public MiscellaneousDataController(IMiscellaneousDataService miscellaneousDataService)
    {
        _miscellaneousDataService = miscellaneousDataService;
    }
    [HttpGet("GetMainMenuMiscellaneousData")]
    public async Task<IActionResult> GetMainMenuMiscellaneousData()
    {
        var miscellaneousData = await _miscellaneousDataService.GetMainMenuMiscellaneousData();
        return Ok(miscellaneousData);
    }
    [HttpGet("GetSettingsMiscellaneousData")]
    public async Task<IActionResult> GetSettingsMiscellaneousData()
    {
        var miscellaneousData = await _miscellaneousDataService.GetSettingsMiscellaneousData();
        return Ok(miscellaneousData);
    }
    [HttpGet("GetMuseumMiscellaneousData")]
    public async Task<IActionResult> GetMuseumMiscellaneousData()
    {
        var miscellaneousData = await _miscellaneousDataService.GetMuseumMiscellaneousData();
        return Ok(miscellaneousData);
    }
    [HttpGet("GetMineMiscellaneousData")]
    public async Task<IActionResult> GetMineMiscellaneousData()
    {
        var miscellaneousData = await _miscellaneousDataService.GetMineMiscellaneousData();
        return Ok(miscellaneousData);
    }
    
}