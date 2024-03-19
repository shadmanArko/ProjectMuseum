using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.SpecialRepositoryService;

public interface ISpecialBackdropService
{
    Task<List<SpecialBackdropPngInformation>> SetSpecialBackdropFromDatabase();
}