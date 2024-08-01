using System;
using System.IO;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.SaveLoadServices;

public static class SaveLoadService
{
    
    
    public static void Save(SaveData saveData)
    {
        string gameDataFolderPath = DataPath.GetSaveDataFolderPath();

        // Ensure the directory exists
        if (!Directory.Exists(gameDataFolderPath))
        {
            Directory.CreateDirectory(gameDataFolderPath);
        }

        try
        {
            // Serialize the data
            string jsonData = JsonConvert.SerializeObject(saveData, Formatting.Indented);

            // Construct the full path for the save file
            string saveFilePath = Path.Combine(gameDataFolderPath, DataPath.SaveFileName);

            // Write the data to the file
            File.WriteAllText(saveFilePath, jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

    public static SaveData Load()
    {
        try
        {
            if (File.Exists(DataPath.SaveDataFilePath))
            {
                string jsonData = File.ReadAllText(DataPath.SaveDataFilePath);
                return JsonConvert.DeserializeObject<SaveData>(jsonData);
            }
            else
            {
                return new SaveData(); // Return a new instance if the file doesn't exist
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
            return new SaveData(); // Return a new instance on error
        }
    }
}