using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineArtifactRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services;

public class MineArtifactService : IMineArtifactService
{
    private readonly IMineArtifactRepository _mineArtifactRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public MineArtifactService(IMineArtifactRepository mineArtifactRepository, IInventoryRepository inventoryRepository)
    {
        _mineArtifactRepository = mineArtifactRepository;
        _inventoryRepository = inventoryRepository;
    }
    public async Task<List<Artifact>?> GetAllArtifactsOfMine()
    {
        var artifacts = await _mineArtifactRepository.GetAllArtifacts();
        return artifacts;
    }

    public async Task<Artifact> SendArtifactToInventory(string artifactId)
    {
        var artifact = await _mineArtifactRepository.RemoveArtifactById(artifactId);
        await _inventoryRepository.AddArtifact(artifact);
        return artifact;
    }

    public async Task<List<Artifact>> GenerateNewArtifacts()
    {
        var listOfArtifacts = new List<Artifact>();
        return listOfArtifacts;
    }
    
}