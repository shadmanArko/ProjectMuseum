using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ProceduralMineGenerationRepository;

public class ProceduralMineGenerationRepository : IProceduralMineGenerationRepository
{
    private readonly JsonFileDatabase<ProceduralMineGenerationData> _proceduralMineGenerationDatabase;

    public ProceduralMineGenerationRepository(JsonFileDatabase<ProceduralMineGenerationData> proceduralMineGenerationDatabase)
    {
        _proceduralMineGenerationDatabase = proceduralMineGenerationDatabase;
    }
}