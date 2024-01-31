using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MiscellaneousDataRepository;

public class MiscellaneousDataRepository: IMiscellaneousDataRepository
{
    private readonly JsonFileDatabase<MainMenuMiscellaneousData> _mainMenuMiscellaneousDatabase;
    private readonly JsonFileDatabase<SettingsMiscellaneousData> _settingsMiscellaneousDatabase;
    private readonly JsonFileDatabase<MuseumMiscellaneousData> _museumMiscellaneousDatabase;
    private readonly JsonFileDatabase<MineMiscellaneousData> _mineMiscellaneousDatabase;

    public MiscellaneousDataRepository(JsonFileDatabase<MainMenuMiscellaneousData> mainMenuMiscellaneousDatabase, JsonFileDatabase<SettingsMiscellaneousData> settingsMiscellaneousDatabase, JsonFileDatabase<MuseumMiscellaneousData> museumMiscellaneousDatabase, JsonFileDatabase<MineMiscellaneousData> mineMiscellaneousDatabase)
    {
        _mainMenuMiscellaneousDatabase = mainMenuMiscellaneousDatabase;
        _settingsMiscellaneousDatabase = settingsMiscellaneousDatabase;
        _museumMiscellaneousDatabase = museumMiscellaneousDatabase;
        _mineMiscellaneousDatabase = mineMiscellaneousDatabase;
    }
    public async Task<MainMenuMiscellaneousData> GetMainMenuMiscellaneousData()
    {
        var miscellaneousDatas = await _mainMenuMiscellaneousDatabase.ReadDataAsync();
        
        return miscellaneousDatas![0];
    }

    public async Task<SettingsMiscellaneousData> GetSettingsMiscellaneousData()
    {
        var miscellaneousDatas = await _settingsMiscellaneousDatabase.ReadDataAsync();
        
        return miscellaneousDatas![0];
    }

    public async Task<MuseumMiscellaneousData> GetMuseumMiscellaneousData()
    {
        var miscellaneousDatas = await _museumMiscellaneousDatabase.ReadDataAsync();
        
        return miscellaneousDatas![0];
    }

    public async Task<MineMiscellaneousData> GetMineMiscellaneousData()
    {
        var miscellaneousDatas = await _mineMiscellaneousDatabase.ReadDataAsync();
        
        return miscellaneousDatas![0];
    }
}