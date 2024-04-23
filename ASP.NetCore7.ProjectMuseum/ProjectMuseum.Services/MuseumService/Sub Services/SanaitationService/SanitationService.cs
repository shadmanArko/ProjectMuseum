using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.SanitationRepository;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.SanaitationService;

public class SanitationService: ISanitationService
{
    private ISanitationRepository _sanitationRepository;

    public SanitationService(ISanitationRepository sanitationRepository)
    {
        _sanitationRepository = sanitationRepository;
    }
    public async Task<List<SanitationVariation>?> GetAllSanitationVariations()
    {
        return await _sanitationRepository.GetAllSanitationVariations();
    }

    public async Task<SanitationVariation?> GetSanitationVariation(string variationName)
    {
        return await _sanitationRepository.GetSanitationVariation(variationName);

    }

    public async Task<List<Sanitation>?> DeleteAllSanitations()
    {
        return await _sanitationRepository.DeleteAllSanitation();

    }

    public async Task<List<Sanitation>?> GetAllSanitations()
    {
        return await _sanitationRepository.GetAllSanitation();

    }
}