using ProjectMuseum.Models;

namespace ProjectMuseum.Services.ExhibitService;

public interface IExhibitService
{
    Task<List<ExhibitVariation>?> GetAllExhibitVariations();
    Task<ExhibitVariation?> GetExhibitVariation(string variationName);
    Task<List<Exhibit>?> DeleteAllExhibits();
    Task<List<Exhibit>?> GetAllExhibits();
    Task<Exhibit?> AddArtifactToExhibit(string exhibitId, string artifactId, int slot);
    Task<Exhibit?> RemoveArtifactFromExhibit(string exhibitId, string artifactId, int slot);
}