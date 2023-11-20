using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.ExhibitRepository;

public interface IExhibitRepository
{
    Task<Exhibit> Insert(Exhibit exhibit);
    Task<Exhibit> Update(string id, Exhibit exhibit);
    Task<Exhibit?> GetById(string id);
    Task<List<Exhibit>?> GetAll();
    Task<List<ExhibitVariation>?> GetAllExhibitVariations();
    Task<Exhibit?> Delete(string id);
}