using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ProceduralMineGenerationRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService;

public class ProceduralMineGenerationService : IProceduralMineGenerationService
{
    private readonly IProceduralMineGenerationRepository _proceduralMineGenerationRepository;
    
    public ProceduralMineGenerationService(IProceduralMineGenerationRepository proceduralMineGenerationRepository)
    {
        _proceduralMineGenerationRepository = proceduralMineGenerationRepository;
    }
    
    public Task GenerateMineOrdinaryCells()
    {
        throw new NotImplementedException();
    }

    public Task GenerateBossCave()
    {
        throw new NotImplementedException();
    }

    public Task GenerateCaves()
    {
        throw new NotImplementedException();
    }

    public Task GenerateSpecialBackdrops()
    {
        throw new NotImplementedException();
    }

    public Task GenerateArtifacts()
    {
        throw new NotImplementedException();
    }

    public Task GenerateResources()
    {
        throw new NotImplementedException();
    }

    public Task GenerateUnbreakableRocks()
    {
        throw new NotImplementedException();
    }
}