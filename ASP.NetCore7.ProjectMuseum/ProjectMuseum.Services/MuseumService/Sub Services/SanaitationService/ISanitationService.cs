using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.SanaitationService;

public interface ISanitationService
{
    Task<List<SanitationVariation>?> GetAllSanitationVariations();
    Task<SanitationVariation?> GetSanitationVariation(string variationName);
    Task<List<Sanitation>?> DeleteAllSanitations();
    Task<List<Sanitation>?> GetAllSanitations();
}