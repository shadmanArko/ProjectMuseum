using ProjectMuseum.Models;

namespace ProjectMuseum.Services.ExhibitService;

public interface IExhibitService
{
    Task<List<ExhibitVariation>?> GetAllExhibitVariations();
    Task<List<Exhibit>?> DeleteAllExhibits();
    Task<List<Exhibit>?> GetAllExhibits();
}