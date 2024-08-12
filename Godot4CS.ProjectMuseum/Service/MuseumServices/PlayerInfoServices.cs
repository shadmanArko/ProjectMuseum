using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class PlayerInfoServices: Node
{
    private MuseumRunningDataContainer _museumRunningDataContainer;
    public override void _Ready()
    {
        _museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
        
        _museumRunningDataContainer.PlayerInfo = SaveLoadService.Load().PlayerInfo;
        base._Ready();
    }

    public PlayerInfo Insert(PlayerInfo playerInfo)
    {
        _museumRunningDataContainer.PlayerInfo = playerInfo;
        return playerInfo;
    }

    public PlayerInfo Update(string id, PlayerInfo playerInfo)
    {
        throw new NotImplementedException();
    }

    public PlayerInfo GetById(string id)
    {
        throw new NotImplementedException();
    }

    public PlayerInfo GetLastPlayerInfo()
    {
        return _museumRunningDataContainer.PlayerInfo;
    }

    public void LoadDataForNewGame(PlayerInfo playerInfo)
    {
        SaveData saveData = new SaveData();
        var exhibitsJson = Godot.FileAccess.Open("res://Game Data/Starting Data/exhibit.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
        var museumTileJson = Godot.FileAccess.Open("res://Game Data/Starting Data/museumTile.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
        var displayArtifactsJson = Godot.FileAccess.Open("res://Game Data/Starting Data/displayArtifact.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
        var artifactsStorageJson = Godot.FileAccess.Open("res://Game Data/Starting Data/artifactStorage.json", Godot.FileAccess.ModeFlags.Read).GetAsText();

        
        saveData.Exhibits = JsonSerializer.Deserialize<List<Exhibit>>(exhibitsJson);
        saveData.MuseumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(museumTileJson);
        saveData.DisplayArtifacts = JsonSerializer.Deserialize<DisplayArtifacts>(displayArtifactsJson);
        saveData.ArtifactStorage = JsonSerializer.Deserialize<ArtifactStorage>(artifactsStorageJson);
        saveData.PlayerInfo = playerInfo;
        
        SaveLoadService.Save(saveData);
    }

    public PlayerInfo UpdateCompletedStory(int completedStoryNumber)
    {
        var playerInfo =  _museumRunningDataContainer.PlayerInfo;
        playerInfo!.CompletedStoryScene = completedStoryNumber;
        return  playerInfo;
    }

    public PlayerInfo UpdateCompletedTutorial(int completedTutorialNumber)
    {
        var playerInfo =  _museumRunningDataContainer.PlayerInfo;
        playerInfo!.CompletedTutorialScene = completedTutorialNumber;
        return  playerInfo;
    }
}