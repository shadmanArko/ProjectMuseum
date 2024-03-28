using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.GuestBuildingParameterRepository;

public interface IGuestBuildingParameterRepository
{
    Task<GuestBuildingParameter> GetGuestBuildingParameter();
}