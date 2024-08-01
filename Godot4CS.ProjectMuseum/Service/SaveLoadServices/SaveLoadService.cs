using System;
using System.IO;
using Newtonsoft.Json;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.SaveLoadServices;

public static class SaveLoadService
{
    private static string filePath = "save.json";
    
    public static void Save(SaveData saveData)
    {
        try
        {
            string jsonData = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
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
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
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