using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class PlayerInfoServices: Node
{
    private List<PlayerInfo> _playerInfoDatabase;

    public override void _Ready()
    {
        var playerInfoDatabaseJson = File.ReadAllText(
            "E:/Godot Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Dummy Data Folder/PlayerInfo.json");
        _playerInfoDatabase = JsonSerializer.Deserialize<List<PlayerInfo>>(playerInfoDatabaseJson);
        base._Ready();
    }

    public PlayerInfo Insert(PlayerInfo playerInfo)
    {
        var playerInfos =  _playerInfoDatabase;
        playerInfos?.Add(playerInfo);
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
        var playerInfos =  _playerInfoDatabase;
        return playerInfos?.Last();
    }

    public PlayerInfo UpdateCompletedStory(int completedStoryNumber)
    {
        var playerInfos =  _playerInfoDatabase;
        var currentPlayerInfo = playerInfos?.Last();
        currentPlayerInfo!.CompletedStoryScene = completedStoryNumber;
        return  currentPlayerInfo;
    }

    public PlayerInfo UpdateCompletedTutorial(int completedTutorialNumber)
    {
        var playerInfos =  _playerInfoDatabase;
        var currentPlayerInfo = playerInfos?.Last();
        currentPlayerInfo!.CompletedTutorialScene = completedTutorialNumber;
        return  currentPlayerInfo;
    }
}