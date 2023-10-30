using ProjectMuseum.Repositories;

namespace ProjectMuseum.Services.LoadAndSaveService;

public class SaveService: ISaveService
{
    private readonly SaveDataJsonFileDatabase _saveDataJsonFileDatabase;

    public SaveService(SaveDataJsonFileDatabase saveDataJsonFileDatabase)
    {
        _saveDataJsonFileDatabase = saveDataJsonFileDatabase;
    }

    public async Task SaveData()
    {
        await _saveDataJsonFileDatabase.MergeJsonFiles();
    }
}