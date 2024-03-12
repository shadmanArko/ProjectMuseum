namespace ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService;

public interface IProceduralMineGenerationService
{
    Task GenerateMineOrdinaryCells();
    Task GenerateBossCave();
    Task GenerateCaves();
    Task GenerateSpecialBackdrops();
    Task GenerateArtifacts();
    Task GenerateResources();
    Task GenerateUnbreakableRocks();
}