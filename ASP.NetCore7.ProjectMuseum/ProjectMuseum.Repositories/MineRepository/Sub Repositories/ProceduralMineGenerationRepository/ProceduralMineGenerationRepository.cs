using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ProceduralMineGenerationRepository;

public class ProceduralMineGenerationRepository : IProceduralMineGenerationRepository
{
    private readonly JsonFileDatabase<ProceduralMineGenerationData> _proceduralMineGenerationDatabase;

    public ProceduralMineGenerationRepository(JsonFileDatabase<ProceduralMineGenerationData> proceduralMineGenerationDatabase)
    {
        _proceduralMineGenerationDatabase = proceduralMineGenerationDatabase;
    }


    public async Task<ProceduralMineGenerationData> GetProceduralMineGenerationData()
    {
        var proceduralMineGenerationDataList = await _proceduralMineGenerationDatabase.ReadDataAsync();
        var proceduralMineGenerationData = proceduralMineGenerationDataList?[0];
        return proceduralMineGenerationData!;
    }
}