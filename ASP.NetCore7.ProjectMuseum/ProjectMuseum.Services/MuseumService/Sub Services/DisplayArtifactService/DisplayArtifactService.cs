using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.DisplayArtifactRepository;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.DisplayArtifactService;

public class DisplayArtifactService : IDisplayArtifactService
{
    private readonly IDisplayArtifactRepository _displayArtifactRepository;

    public DisplayArtifactService(IDisplayArtifactRepository displayArtifactRepository)
    {
        _displayArtifactRepository = displayArtifactRepository;
    }

    public async Task<List<Artifact>?> GetAllArtifacts()
    {
        var artifacts = await _displayArtifactRepository.GetAllArtifacts();
        return artifacts;
    }

    public async Task<Artifact?> GetArtifactById(string id)
    {
        var artifact = await _displayArtifactRepository.GetArtifactById(id);
        return artifact;
    }

    public async Task<Artifact?> AddArtifact(Artifact artifact)
    {
       return await _displayArtifactRepository.AddArtifact(artifact);
    }
    public async Task<Artifact?> RemoveArtifactById(string id)
    {
        return await _displayArtifactRepository.RemoveArtifactById(id);
    }
}