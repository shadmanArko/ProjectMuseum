using ProjectMuseum.Repositories;

namespace ProjectMuseum.Services.LoadAndSaveService;

public class LoadService: ILoadService
{
    private readonly SaveDataJsonFileDatabase _saveDataJsonFileDatabase;

    public LoadService(SaveDataJsonFileDatabase saveDataJsonFileDatabase)
    {
        _saveDataJsonFileDatabase = saveDataJsonFileDatabase;
    }

    public async Task LoadData()
    {
        await _saveDataJsonFileDatabase.SplitJsonFile();
    }
}