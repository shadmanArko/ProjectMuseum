using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.SpecialBackdropRepository;
using ProjectMuseum.Services.MineService;
using ProjectMuseum.Services.MineService.Sub_Services;
using ProjectMuseum.Services.MineService.Sub_Services.CaveService;
using ProjectMuseum.Services.MineService.Sub_Services.MineCellCrackService;
using ProjectMuseum.Services.MineService.Sub_Services.MineCellService;
using ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService;
using ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService.MineOrdinaryCellGeneratorService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService.RawArtifactDescriptiveService;
using ProjectMuseum.Services.MineService.Sub_Services.ResourceService;
using ProjectMuseum.Services.MineService.Sub_Services.SpecialBackdropService;
using ProjectMuseum.Services.MineService.Sub_Services.WallPlaceableService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MineController : ControllerBase
{
    private readonly IMineService _mineService;
    private readonly IMineArtifactService _mineArtifactService;
    private readonly IMineOrdinaryCellGeneratorService _mineOrdinaryCellGeneratorService;
    private readonly IMineCellCrackMaterialService _mineCellCrackMaterialService;
    private readonly IRawArtifactFunctionalService _rawArtifactFunctionalService;
    private readonly IRawArtifactDescriptiveService _rawArtifactDescriptiveService;
    private readonly IResourceService _resourceService;
    private readonly IWallPlaceableService _wallPlaceableService;
    private readonly ICaveGeneratorService _caveGeneratorService;
    private readonly ISpecialBackdropService _specialBackdropService;
    private readonly IProceduralMineGenerationService _proceduralMineGenerationService;

    public MineController(IMineService mineService, IMineArtifactService mineArtifactService, IMineOrdinaryCellGeneratorService mineOrdinaryCellGeneratorService, IMineCellCrackMaterialService mineCellCrackMaterialService, IRawArtifactDescriptiveService rawArtifactDescriptiveService, IRawArtifactFunctionalService rawArtifactFunctionalService, IResourceService resourceService, IWallPlaceableService wallPlaceableService, ICaveGeneratorService caveGeneratorService, ISpecialBackdropService specialBackdropService, IProceduralMineGenerationService proceduralMineGenerationService)
    {
        _mineService = mineService;
        _mineArtifactService = mineArtifactService;
        _mineOrdinaryCellGeneratorService = mineOrdinaryCellGeneratorService;
        _mineCellCrackMaterialService = mineCellCrackMaterialService;
        _rawArtifactDescriptiveService = rawArtifactDescriptiveService;
        _rawArtifactFunctionalService = rawArtifactFunctionalService;
        _resourceService = resourceService;
        _wallPlaceableService = wallPlaceableService;
        _caveGeneratorService = caveGeneratorService;
        _specialBackdropService = specialBackdropService;
        _proceduralMineGenerationService = proceduralMineGenerationService;
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

    // [HttpGet("GenerateMine")]
    // public async Task<IActionResult> GenerateMine()
    // {
    //     var mine = await _mineOrdinaryCellGeneratorService.GenerateMineCellData();
    //     return Ok(mine);
    // }
    
    [HttpGet("GenerateProceduralMine")]
    public async Task<IActionResult> GenerateProceduralMine()
    {
        var mine = await _proceduralMineGenerationService.GenerateProceduralMine();
        return Ok(mine);
    }

    [HttpGet("AssignArtifactsToMine")]
    public async Task<IActionResult> AssignArtifactsToMine()
    {
        var mine = await _mineService.AssignArtifactsToMine();
        return Ok(mine);
    }
    
    [HttpGet("AssignResourcesToMine")]
    public async Task<IActionResult> AssignResourcesToMine()
    {
        var mine = await _resourceService.AssignResourcesToMine();
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
    
    [HttpGet("SendResourceFromMineToInventory/{resourceId}")]
    public async Task<IActionResult> SendResourceFromMineToInventory(string resourceId)
    {
        var inventoryItem = await _resourceService.SendResourceFromMineToInventory(resourceId);
        return Ok(inventoryItem);
    }
    
    [HttpGet("SendWallPlaceableFromMineToInventory/{wallPlaceableId}")]
    public async Task<IActionResult> SendWallPlaceableFromMineToInventory(string wallPlaceableId)
    {
        var wallPlaceable = await _wallPlaceableService.SendWallPlaceableFromMineToInventory(wallPlaceableId);
        return Ok(wallPlaceable);
    }

    [HttpGet("GetWallPlaceableByVariant/{variant}")]
    public async Task<IActionResult> GetWallPlaceableByVariant(string variant)
    {
        var wallPlaceable = await _wallPlaceableService.GetWallPlaceableByVariant(variant);
        return Ok(wallPlaceable);
    }
    
    [HttpGet("AddCave")]
    public async Task<IActionResult> GenerateCave()
    {
        var cave = await _caveGeneratorService.GenerateCave(20,30,10,15, 5, 5);
        return Ok(cave);
    }
    
    [HttpGet("GenerateResources")]
    public async Task<IActionResult> GenerateResources()
    {
        var mine = await _resourceService.GenerateResources();
        return Ok(mine);
    }
    
    // [HttpGet("SetSpecialBackdropsInMine")]
    // public async Task<IActionResult> SetSpecialBackdropsInMine()
    // {
    //     var specialBackdrops = await _specialBackdropService.SetSpecialBackdrops();
    //     return Ok(specialBackdrops);
    // }
}