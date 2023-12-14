using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineArtifactRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services;

public class MineArtifactService : IMineArtifactService
{
    private readonly IMineService _mineService;
    private readonly IMineArtifactRepository _mineArtifactRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public MineArtifactService(IMineArtifactRepository mineArtifactRepository, IInventoryRepository inventoryRepository, IMineService mineService)
    {
        _mineArtifactRepository = mineArtifactRepository;
        _inventoryRepository = inventoryRepository;
        _mineService = mineService;
    }
    public async Task<List<Artifact>?> GetAllArtifactsOfMine()
    {
        var artifacts = await _mineArtifactRepository.GetAllArtifacts();
        return artifacts;
    }

    public async Task<Artifact> SendArtifactToInventory(string artifactId)
    {
        var artifact = await _mineArtifactRepository.RemoveArtifactById(artifactId);
        var mine = await _mineService.GetMineData();

        mine.Cells.FirstOrDefault(tempCell => tempCell.ArtifactId == artifact.Id)!.HasArtifact = false;
        await _mineService.UpdateMine(mine);
        await _inventoryRepository.AddArtifact(artifact);
        return artifact;
    }

    public async Task<List<Artifact>> GenerateNewArtifacts()
    {
        var listOfArtifacts = new List<Artifact>();
        return listOfArtifacts;
    }

    public async Task<Artifact?> GetArtifactById(string id)
    {
        return await _mineArtifactRepository.GetArtifactById(id);
    }
}