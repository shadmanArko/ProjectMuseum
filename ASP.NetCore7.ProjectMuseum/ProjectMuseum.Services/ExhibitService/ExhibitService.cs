using ProjectMuseum.Models;
using ProjectMuseum.Repositories.ExhibitRepository;

namespace ProjectMuseum.Services.ExhibitService;

public class ExhibitService : IExhibitService
{
    private IExhibitRepository _exhibitRepository;

    public ExhibitService(IExhibitRepository exhibitRepository)
    {
        _exhibitRepository = exhibitRepository;
    }
    public async Task<List<ExhibitVariation>?> GetAllExhibitVariations()
    {
        return await _exhibitRepository.GetAllExhibitVariations();
    }

    public async Task<List<Exhibit>?> DeleteAllExhibits()
    {
        return await _exhibitRepository.DeleteAllExhibits();
    }

    public async Task<List<Exhibit>?> GetAllExhibits()
    {
        return await _exhibitRepository.GetAllExhibits();
    }

    public async Task<Exhibit?> AddArtifactToExhibit(string exhibitId, string artifactId, int slot)
    {
        return await _exhibitRepository.AddArtifactToExhibit(exhibitId, artifactId, slot);
    }

    public async Task<Exhibit?> RemoveArtifactFromExhibit(string exhibitId, string artifactId, int slot)
    {
        return await _exhibitRepository.RemoveArtifactFromExhibit(exhibitId, artifactId, slot);
    }
}