using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.GuestBuildingParameterRepository;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.GuestBuilderParameterService;

public class GuestBuilderParameterService: IGuestBuilderParameterService
{
    private IGuestBuildingParameterRepository _guestBuildingParameterRepository;

    public GuestBuilderParameterService(IGuestBuildingParameterRepository guestBuildingParameterRepository)
    {
        _guestBuildingParameterRepository = guestBuildingParameterRepository;
    }

    public async Task<GuestBuildingParameter> GetGuestBuildingParameter()
    {
        return await _guestBuildingParameterRepository.GetGuestBuildingParameter();
    }
}