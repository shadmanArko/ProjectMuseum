using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;

namespace ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService;

public interface IProceduralMineGenerationService
{
    Task<Mine> GenerateProceduralMine();
    Task GenerateMineOrdinaryCells();
    Task GenerateBossCave();
    Task GenerateCaves();
    Task GenerateSpecialBackdrops();
    Task GenerateArtifacts();
    Task<List<Resource>> GenerateResources();
    Task GenerateBoulders();
}