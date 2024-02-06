using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MiscellaneousDataRepository;

public interface IMiscellaneousDataRepository
{
    Task<MainMenuMiscellaneousData> GetMainMenuMiscellaneousData();
    Task<SettingsMiscellaneousData> GetSettingsMiscellaneousData();
    Task<MuseumMiscellaneousData> GetMuseumMiscellaneousData();
    Task<MineMiscellaneousData> GetMineMiscellaneousData();
}