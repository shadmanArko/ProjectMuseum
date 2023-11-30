using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineArtifactRepository;

namespace ProjectMuseum.Services.MineService;

public class MineService : IMineService
{
    private readonly IMineRepository _mineRepository;
    private readonly IMineArtifactRepository _mineArtifactRepository;

    public MineService(IMineRepository mineRepository, IMineArtifactRepository mineArtifactRepository)
    {
        _mineRepository = mineRepository;
        _mineArtifactRepository = mineArtifactRepository;
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
                }
            }
        }
        
        await _mineRepository.Update(mine);
        return mine;
    }
}