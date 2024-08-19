using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.SaveLoadServices;

public class SaveLoadDemo
{
    private SaveData _saveData;

    public SaveLoadDemo(SaveData saveData)
    {
        _saveData = saveData;
    }

    public void SaveGame()
    {
        SaveLoadService.Save(_saveData);
    }

    public void LoadGame()
    {
        _saveData = SaveLoadService.Load();
    }
}