using ProjectMuseum.Models.MIne;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VineInformationRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.VineInformationService;

public class VineInformationService : IVineInformationService
{
    private IVineInformationRepository _vineInformationRepository;

    public VineInformationService(IVineInformationRepository vineInformationRepository)
    {
        _vineInformationRepository = vineInformationRepository;
    }

    public async Task<List<VineInformation>> SetVineBackdrops(List<VineInformation> vineInfos)
    {
        var vineInformations = await _vineInformationRepository.SetVineBackdrops(vineInfos);
        return vineInformations;
    }
}