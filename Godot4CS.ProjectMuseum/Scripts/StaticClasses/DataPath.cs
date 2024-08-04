using System;
using System.IO;

namespace Godot4CS.ProjectMuseum.Scripts.StaticClasses;

public static class DataPath
{
    public const string GameDataFolderPath = "user://MuseumKeeper/GameData";
    public const string GameDataFilePath = "";
    public const string GameDataSourceFilePath = "";
    private const string GameFolderName = "MuseumKeeper";
    private const string SaveDataFolderName = "SaveData";
    
    // public const string SaveDataFolderPath = "user://MuseumKeeper/SaveData";
    public static string SaveDataFolderPath => GetSaveDataFolderPath();
    public static string SaveDataFilePath => GetSaveDataFilePath();
    public const string SaveFileName = "Save01.json";
    
    public const string RunningDataFolderPath = "";
    public const string RunningDataFilePath = "";

    private static string GetSaveDataFolderPath()
    {
        string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        

        // Combine paths to create the full directory path
        return Path.Combine(userFolderPath, GameFolderName, SaveDataFolderName);
    }
    private static string GetSaveDataFilePath()
    {
        // Combine paths to create the full directory path
        return Path.Combine(SaveDataFolderPath, SaveFileName);
    }
    public static void OnGameStart()
    {
        //check for gameData folder existence 
        if (!Directory.Exists(GameDataFolderPath))
        {
            Directory.CreateDirectory(GameDataFolderPath);
        }
        
        //check for gameData file existence
        try
        {
            File.Copy(GameDataSourceFilePath, GameDataFilePath, false);
        }
        catch (IOException)
        {
            Console.WriteLine("Found GameData File");
        }
        
        //check for saveData folder existence
        if (!Directory.Exists(GameDataFolderPath))
        {
            Directory.CreateDirectory(GameDataFolderPath);
        }
        
        //get all the save files and check it is real save file or not
    }
}