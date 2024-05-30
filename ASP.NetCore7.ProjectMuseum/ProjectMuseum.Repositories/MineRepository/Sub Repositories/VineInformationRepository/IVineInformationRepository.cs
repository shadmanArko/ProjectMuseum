using ProjectMuseum.Models.MIne;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VineInformationRepository;

public interface IVineInformationRepository
{
    Task<List<VineInformation>> SetVineBackdrops(List<VineInformation> vineInformations);
}