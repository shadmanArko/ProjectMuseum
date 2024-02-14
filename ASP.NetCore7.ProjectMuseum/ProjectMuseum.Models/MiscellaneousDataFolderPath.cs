using System.Text.Json;

namespace ProjectMuseum.Models;

public static class MiscellaneousDataFolderPath
{
    public static string MainMenuMiscellaneousDataFolderPath(string language)
    {
        string mainMenuMiscellaneousDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Game Data Folder", "MiscellaneousData", "MainMenuMiscellaneousData", $"MainMenuMiscellaneousData{language}.json");
        return mainMenuMiscellaneousDataFolderPath;
    }
    
}