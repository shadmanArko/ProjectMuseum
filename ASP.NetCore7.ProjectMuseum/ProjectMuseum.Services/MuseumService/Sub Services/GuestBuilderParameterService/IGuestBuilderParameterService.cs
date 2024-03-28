using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.GuestBuilderParameterService;

public interface IGuestBuilderParameterService
{
    Task<GuestBuildingParameter> GetGuestBuildingParameter();
}