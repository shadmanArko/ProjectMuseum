using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MiscellaneousDataRepository;

namespace ProjectMuseum.Services.MiscellaneousDataService;

public class MiscellaneousDataService: IMiscellaneousDataService
{
    private readonly IMiscellaneousDataRepository _miscellaneousDataRepository;

    public MiscellaneousDataService(IMiscellaneousDataRepository miscellaneousDataRepository)
    {
        _miscellaneousDataRepository = miscellaneousDataRepository;
    }
    public Task<MainMenuMiscellaneousData> GetMainMenuMiscellaneousData()
    {
        return _miscellaneousDataRepository.GetMainMenuMiscellaneousData();
    }

    public Task<SettingsMiscellaneousData> GetSettingsMiscellaneousData()
    {
        return _miscellaneousDataRepository.GetSettingsMiscellaneousData();
    }

    public Task<MuseumMiscellaneousData> GetMuseumMiscellaneousData()
    {
        return _miscellaneousDataRepository.GetMuseumMiscellaneousData();
    }

    public Task<MineMiscellaneousData> GetMineMiscellaneousData()
    {
        return _miscellaneousDataRepository.GetMineMiscellaneousData();
    }
}