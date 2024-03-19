using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.SpecialBackdropService;

public interface ISpecialBackdropService
{
    Task<List<SpecialBackdropPngInformation>> SetSpecialBackdrops(List<SpecialBackdropPngInformation> specialBackdropPngInformations);
}