using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.GuestBuildingParameterRepository;

public class GuestBuildingParameterRepository: IGuestBuildingParameterRepository
{
    private readonly JsonFileDatabase<GuestBuildingParameter> _guestBuildingParameter;

    public GuestBuildingParameterRepository(JsonFileDatabase<GuestBuildingParameter> guestBuildingParameter)
    {
        _guestBuildingParameter = guestBuildingParameter;
    }
    public async Task<GuestBuildingParameter> GetGuestBuildingParameter()
    {
        var guestBuildingParameters = await _guestBuildingParameter.ReadDataAsync();
        return guestBuildingParameters![0];
    }
}