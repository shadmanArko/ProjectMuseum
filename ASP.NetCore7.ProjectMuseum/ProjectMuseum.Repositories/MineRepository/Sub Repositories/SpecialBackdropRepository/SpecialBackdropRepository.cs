using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.SpecialBackdropRepository;

public class SpecialBackdropRepository : ISpecialBackdropRepository
{
    private readonly JsonFileDatabase<SpecialBackdropPngInformation> _specialBackdropDatabase;
    private readonly IMineRepository _mineRepository;

    public SpecialBackdropRepository(JsonFileDatabase<SpecialBackdropPngInformation> specialBackdropDatabase, IMineRepository mineRepository)
    {
        _specialBackdropDatabase = specialBackdropDatabase;
        _mineRepository = mineRepository;
    }

    public async Task<List<SpecialBackdropPngInformation>> SetSpecialBackdrops(List<SpecialBackdropPngInformation> specialBackdrops)
    {
        var mine = await _mineRepository.Get();
        mine.SpecialBackdropPngInformations = specialBackdrops;
        await _mineRepository.Update(mine);
        return specialBackdrops;
    }
}