using ProjectMuseum.Models.MIne;

namespace ProjectMuseum.Services.MineService.Sub_Services.VineInformationService;

public interface IVineInformationService
{
    Task<List<VineInformation>> SetVineBackdrops(List<VineInformation> vineInformations);
}