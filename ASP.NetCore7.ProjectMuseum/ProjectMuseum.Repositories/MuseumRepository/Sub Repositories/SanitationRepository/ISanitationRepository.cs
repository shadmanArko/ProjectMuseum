using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.SanitationRepository;

public interface ISanitationRepository
{
    Task<Sanitation> Insert(Sanitation sanitation);
    Task<Sanitation> Update(string id, Sanitation sanitation);
    Task<Sanitation?> GetById(string id);
    Task<List<Sanitation>?> GetAll();
    Task<List<SanitationVariation>?> GetAllSanitationVariations();
    Task<SanitationVariation?> GetSanitationVariation(string variationName);
    Task<List<Sanitation>?> GetAllSanitation();
    Task<Sanitation?> Delete(string id);
    Task<List<Sanitation>?> DeleteAllSanitation();
}