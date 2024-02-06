using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MiscellaneousDataService;

public interface IMiscellaneousDataService
{
    Task<MainMenuMiscellaneousData> GetMainMenuMiscellaneousData();
    Task<SettingsMiscellaneousData> GetSettingsMiscellaneousData();
    Task<MuseumMiscellaneousData> GetMuseumMiscellaneousData();
    Task<MineMiscellaneousData> GetMineMiscellaneousData();
}