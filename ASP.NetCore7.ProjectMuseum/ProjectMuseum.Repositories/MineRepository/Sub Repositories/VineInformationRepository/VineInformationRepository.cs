using ProjectMuseum.Models.MIne;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VineInformationRepository;

public class VineInformationRepository : IVineInformationRepository
{
    private readonly IMineRepository _mineRepository;

    public VineInformationRepository(IMineRepository mineRepository)
    {
        _mineRepository = mineRepository;
    }

    public async Task<List<VineInformation>> SetVineBackdrops(List<VineInformation> vineInformations)
    {
        var mine = await _mineRepository.Get();
        mine.VineInformations = vineInformations;
        await _mineRepository.Update(mine);
        return vineInformations;
    }
}