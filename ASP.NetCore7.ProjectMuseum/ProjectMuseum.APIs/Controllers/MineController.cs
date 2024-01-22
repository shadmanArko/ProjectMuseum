using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Models;
using ProjectMuseum.Services.MineService;
using ProjectMuseum.Services.MineService.Sub_Services;
using ProjectMuseum.Services.MineService.Sub_Services.MineCellCrackService;
using ProjectMuseum.Services.MineService.Sub_Services.MineCellService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService.RawArtifactDescriptiveService;
using ProjectMuseum.Services.MineService.Sub_Services.VehicleService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MineController : ControllerBase
{
    private readonly IMineService _mineService;
    private readonly IMineArtifactService _mineArtifactService;
    private readonly IMineCellGeneratorService _mineCellGeneratorService;
    private readonly IMineCellCrackMaterialService _mineCellCrackMaterialService;
    private readonly IRawArtifactFunctionalService _rawArtifactFunctionalService;
    private readonly IRawArtifactDescriptiveService _rawArtifactDescriptiveService;
    private readonly IVehicleService _vehicleService;

    public MineController(IMineService mineService, IMineArtifactService mineArtifactService, IMineCellGeneratorService mineCellGeneratorService, IMineCellCrackMaterialService mineCellCrackMaterialService, IRawArtifactDescriptiveService rawArtifactDescriptiveService, IRawArtifactFunctionalService rawArtifactFunctionalService, IVehicleService vehicleService)
    {
        _mineService = mineService;
        _mineArtifactService = mineArtifactService;
        _mineCellGeneratorService = mineCellGeneratorService;
        _mineCellCrackMaterialService = mineCellCrackMaterialService;
        _rawArtifactDescriptiveService = rawArtifactDescriptiveService;
        _rawArtifactFunctionalService = rawArtifactFunctionalService;
        _vehicleService = vehicleService;
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
    
    [HttpGet("SendArtifactToInventory/{id}")]
    public async Task<IActionResult> SendArtifactFromMineToInventory(string id)
    {
        var artifact = await _mineArtifactService.SendArtifactToInventory(id);
        return Ok(artifact);
    }

    [HttpGet("GenerateMine")]
    public async Task<IActionResult> GenerateMine()
    {
        var mine = await _mineCellGeneratorService.GenerateMineCellData();
        return Ok(mine);
    }

    [HttpGet("AssignArtifactsToMine")]
    public async Task<IActionResult> AssignArtifactsToMine()
    {
        var mine = await _mineService.AssignArtifactsToMine();
        return Ok(mine);
    }
    
    [HttpGet("GetAllMineArtifacts")]
    public async Task<IActionResult> GetAllMineArtifacts()
    {
        var artifacts = await _mineArtifactService.GetAllArtifactsOfMine();
        return Ok(artifacts);
    }
    
    [HttpGet("GetMineArtifactById/{id}")]
    public async Task<IActionResult> GetMineArtifactById(string id)
    {
        var artifact = await _mineArtifactService.GetArtifactById(id);
        return Ok(artifact);
    }
    
    [HttpGet("GetMineCellCrackMaterial/{materialType}")]
    public async Task<IActionResult> GetMineCellCrackMaterial(string materialType)
    {
        var cellCrackMaterial = await _mineCellCrackMaterialService.GetCellCrackMaterial(materialType);
        return Ok(cellCrackMaterial);
    }

    [HttpGet("AddTutorialArtifactToMine")]
    public async Task<IActionResult> AddTutorialArtifactToMine()
    {
        await _mineService.AssignTutorialArtifactToMine();
        return Ok();
    }

    [HttpGet("GetAllMineCellCrackMaterials")]
    public async Task<IActionResult> GetAllCellCrackMaterials()
    {
        var cellCrackMaterials = await _mineCellCrackMaterialService.GetAllCellCrackMaterials();
        return Ok(cellCrackMaterials);
    }

    [HttpGet("GetAllRawArtifactFunctional")]
    public async Task<IActionResult> GetAllRawArtifactFunctional()
    {
        var listOfRawArtifactFunctional = await _rawArtifactFunctionalService.GetAllRawArtifactFunctional();
        return Ok(listOfRawArtifactFunctional);
    }
    
    [HttpGet("GetAllRawArtifactDescriptive")]
    public async Task<IActionResult> GetAllRawArtifactDescriptive()
    {
        var listOfRawArtifactDescriptive = await _rawArtifactDescriptiveService.GetAllRawArtifactDescriptive();
        return Ok(listOfRawArtifactDescriptive);
    }
    
    [HttpGet("SendVehicleFromMineToInventory/{vehicleId}")]
    public async Task<IActionResult> SendVehicleFromMineToInventory(string vehicleId)
    {
        var equipable = await _vehicleService.SendVehicleToInventory(vehicleId);
        return Ok(equipable);
    }

}