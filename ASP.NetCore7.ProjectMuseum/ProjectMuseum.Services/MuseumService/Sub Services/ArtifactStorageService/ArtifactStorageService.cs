using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;
using ProjectMuseum.Services.MuseumService.Sub_Services.DisplayArtifactService;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactStorageService;

public class ArtifactStorageService : IArtifactStorageService
{
    private readonly IArtifactStorageRepository _artifactStorageRepository;
    private readonly IDisplayArtifactService _displayArtifactService;

    public ArtifactStorageService(IArtifactStorageRepository artifactStorageRepository, IDisplayArtifactService displayArtifactService)
    {
        _artifactStorageRepository = artifactStorageRepository;
        _displayArtifactService = displayArtifactService;
    }

    public async Task<List<Artifact>?> GetAllArtifactsOfStorage()
    {
        return await _artifactStorageRepository.GetAllArtifacts();
    }

    public async Task<Artifact?> SendArtifactToDisplayById(string id)
    {
        var artifact = await _artifactStorageRepository.RemoveArtifactById(id);
        return await _displayArtifactService.AddArtifact(artifact);
    }

    public async Task<Artifact?> GetArtifactOutOfDisplayById(string id)
    {
        var allArtifacts = await _artifactStorageRepository.GetAllArtifacts();
        
        var artifact = await _artifactStorageRepository.AddArtifact(allArtifacts.FirstOrDefault(artifact1 => artifact1.Id == id));
        return await _displayArtifactService.RemoveArtifactById(id);
    }

    public async Task<List<Artifact>?> AddListOfArtifacts(List<Artifact> artifacts)
    {
        return await _artifactStorageRepository.AddListOfArtifacts(artifacts);
    }
}