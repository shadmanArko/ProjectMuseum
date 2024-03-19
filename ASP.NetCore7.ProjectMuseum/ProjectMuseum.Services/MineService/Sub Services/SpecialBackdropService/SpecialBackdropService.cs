using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.SpecialBackdropRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.SpecialBackdropService;

public class SpecialBackdropService : ISpecialBackdropService
{
    private readonly ISpecialBackdropRepository _specialBackdropRepository;

    public SpecialBackdropService(ISpecialBackdropRepository specialBackdropRepository)
    {
        _specialBackdropRepository = specialBackdropRepository;
    }

    public async Task<List<SpecialBackdropPngInformation>> SetSpecialBackdrops(List<SpecialBackdropPngInformation> specialBackdropPngInformations)
    {
        var specialBackdrops = await _specialBackdropRepository.SetSpecialBackdrops(specialBackdropPngInformations);
        return specialBackdrops;
    }
}