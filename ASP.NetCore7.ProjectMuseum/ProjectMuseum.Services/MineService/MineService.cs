using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineArtifactRepository;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService;

namespace ProjectMuseum.Services.MineService;

public class MineService : IMineService
{
    private readonly IMineRepository _mineRepository;
    private readonly IMineArtifactRepository _mineArtifactRepository;
    private readonly IRawArtifactFunctionalService _rawArtifactFunctionalService;

    public MineService(IMineRepository mineRepository, IMineArtifactRepository mineArtifactRepository, IRawArtifactFunctionalService rawArtifactFunctionalService)
    {
        _mineRepository = mineRepository;
        _mineArtifactRepository = mineArtifactRepository;
        _rawArtifactFunctionalService = rawArtifactFunctionalService;
    }

    public async Task<Mine> UpdateMine(Mine mine)
    {
        await _mineRepository.Update(mine);
        return mine;
    }

    public async Task<Mine> GetMineData()
    {
        var mine = await _mineRepository.Get();
        return mine;
    }

    public async Task<Mine> AssignArtifactsToMine()
    {
        var artifacts = await _mineArtifactRepository.GetAllArtifacts();
        var mine = await _mineRepository.Get();

        foreach (var artifact in artifacts)
        {
            foreach (var cell in mine.Cells)
            {
                if (cell.PositionX == artifact.PositionX && cell.PositionY == artifact.PositionY)
                {
                    cell.HasArtifact = true;
                    cell.ArtifactId = artifact.Id;

                    var temp = await _rawArtifactFunctionalService.GetAllRawArtifactFunctional();
                    var rawArtifactMaterial = temp.FirstOrDefault(mat => mat.Id == artifact.RawArtifactId).Materials[0];
                    cell.ArtifactMaterial = rawArtifactMaterial;
                }
            }
        }
        
        await _mineRepository.Update(mine);
        return mine;
    }
}