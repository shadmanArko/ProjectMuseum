using System;
using System.IO;

namespace Godot4CS.ProjectMuseum.Scripts.StaticClasses;

public static class DataPath
{
    public const string GameDataFolderPath = "user://MuseumKeeper/GameData";
    public const string GameDataFilePath = "";
    public const string GameDataSourceFilePath = "";
    
    public const string SaveDataFolderPath = "user://MuseumKeeper/SaveData";
    public const string SaveDataFilePath = "user://MuseumKeeper/SaveData/Save01.json";
    
    public const string RunningDataFolderPath = "";
    public const string RunningDataFilePath = "";

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