using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.ExhibitRepository;

public interface IExhibitRepository
{
    Task<Exhibit> Insert(Exhibit exhibit);
    Task<Exhibit> Update(string id, Exhibit exhibit);
    Task<Exhibit?> GetById(string id);
    Task<List<Exhibit>?> GetAll();
    Task<List<ExhibitVariation>?> GetAllExhibitVariations();
    Task<List<Exhibit>?> GetAllExhibits();
    Task<Exhibit?> Delete(string id);
    Task<List<Exhibit>?> DeleteAllExhibits();
    Task<Exhibit?> AddArtifactToExhibit(string exhibitId, string artifactId, int slot);
    Task<Exhibit?> RemoveArtifactFromExhibit(string exhibitId, string artifactId, int slot);
}