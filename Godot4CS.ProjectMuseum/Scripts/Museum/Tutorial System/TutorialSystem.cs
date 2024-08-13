using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Managers;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Tutorial_System;

public partial class TutorialSystem : Node
{
    private HttpRequest _httpRequestForGettingTutorial;
    private HttpRequest _httpRequestForCompletingTutorial;
    private HttpRequest _httpRequestForGettingPlayerInfo;

    private int _currentTutorialNumber;
    private int _currentTutorialSceneNumber;

    private PlayerInfo _playerInfo; 
    private Tutorial _currentTutorial;
    private TutorialSceneEntry _currentTutorialSceneEntry;
    private bool _currentTutorialCompleted = true;
    private List<string> _performedKeyBinds = new List<string>();
    private List<string> _performedActions = new List<string>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _httpRequestForGettingTutorial = new HttpRequest();
        _httpRequestForGettingPlayerInfo = new HttpRequest();
        _httpRequestForCompletingTutorial = new HttpRequest();
        AddChild(_httpRequestForGettingTutorial);
        AddChild(_httpRequestForGettingPlayerInfo);
        AddChild(_httpRequestForCompletingTutorial);
        // _httpRequestForGettingTutorial.RequestCompleted += HttpRequestForGettingTutorialOnRequestCompleted;
        // _httpRequestForGettingPlayerInfo.RequestCompleted += HttpRequestForGettingPlayerInfoOnRequestCompleted;
        // _httpRequestForCompletingTutorial.RequestCompleted += HttpRequestForCompletingTutorialOnRequestCompleted;
        MuseumActions.OnPlayerPerformedTutorialRequiringAction += OnPlayerPerformedTutorialRequiringAction;
        // _httpRequestForGettingPlayerInfo.Request(ApiAddress.PlayerApiPath + "GetPlayerInfo");
        _playerInfo = SaveLoadService.Load().PlayerInfo;
        
        MuseumActions.PlayTutorial += LoadTutorial;
    }

    private void HttpRequestForCompletingTutorialOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        //GD.Print(jsonStr);
        var playerInfo = JsonSerializer.Deserialize<PlayerInfo>(jsonStr);
        
        //GD.Print($"tutorial completion updated to {playerInfo.CompletedTutorialScene}");
    }

    private void HttpRequestForGettingPlayerInfoOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        //GD.Print( "Player info " +jsonStr);
        _playerInfo = JsonSerializer.Deserialize<PlayerInfo>(jsonStr);
    }

    private void OnPlayerPerformedTutorialRequiringAction(string obj)
    {
        _performedActions.Add(obj);
        if(!_currentTutorialCompleted) CheckForTutorialCompletion();
    }

    void LoadTutorial(int number)
    {
        
        //GD.Print("Show tutorial called");
        _currentTutorialNumber = number;
        // _httpRequestForGettingTutorial.Request(ApiAddress.StoryApiPath + $"GetTutorialScene/{number}");
        _currentTutorial = GetTutorialByNumber(number);
        AfterGettingTutorial();
    }
    
    private Tutorial GetTutorialByNumber(int sceneNo)
    {
        var tutorialsJson = Godot.FileAccess.Open("res://Game Data/TutorialData/Tutorials.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
        var tutorials = JsonSerializer.Deserialize<List<Tutorial>>(tutorialsJson);
        var tutorial = tutorials!.FirstOrDefault(story => story.SceneNo == sceneNo);
        return tutorial;
    }

    private void HttpRequestForGettingTutorialOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        //GD.Print(jsonStr);
        _currentTutorial = JsonSerializer.Deserialize<Tutorial>(jsonStr);
        // AfterGettingTutorial();
        
        
    }

    private void AfterGettingTutorial()
    {
        if (_playerInfo.Tutorial)
        {
            //GD.Print("Show tutorial invoked");
            _currentTutorialSceneNumber = 0;
            _currentTutorialCompleted = false;
            ShowNextTutorialScene();
        }
        else
        {
            MuseumActions.PlayStoryScene?.Invoke(_currentTutorial.StoryNumber);
        }
    }

    private void ShowNextTutorialScene()
    {
        if (_currentTutorialSceneNumber< _currentTutorial.TutorialSceneEntries.Count)
        {
            _currentTutorialSceneEntry = _currentTutorial.TutorialSceneEntries[_currentTutorialSceneNumber];
            _performedKeyBinds.Clear();
            _performedActions.Clear();
            MuseumActions.OnTutorialUpdated?.Invoke(_currentTutorialSceneEntry.TutorialText);
            _currentTutorialSceneNumber++;
        }
        else
        {
            CompleteTutorial();
            //GD.Print($"current tutorial No {_currentTutorialSceneEntry.EntryNo}");
            if (_currentTutorial.ContinuesStory)
            {
                MuseumActions.PlayStoryScene?.Invoke(_currentTutorial.StoryNumber);
            }
        }
        
    }

    private void CompleteTutorial()
    {
        _currentTutorialCompleted = true;
        // _httpRequestForCompletingTutorial.Request(ApiAddress.PlayerApiPath +
        //                                           $"UpdateCompletedTutorial/{_currentTutorialNumber}");
        // MuseumReferenceManager.Instance.PlayerInfoServices.UpdateCompletedTutorial(_currentTutorialNumber);
        _playerInfo.CompletedTutorialScene = _currentTutorialNumber;
        MuseumActions.OnPlayerInfoUpdated?.Invoke(_playerInfo);

        MuseumActions.OnTutorialEnded?.Invoke();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public string GetCurrentTutorialSceneEntry() => _currentTutorialSceneEntry.EntryNo;

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (_currentTutorialSceneEntry== null || _currentTutorialSceneEntry.KeyBindsNeedsToPerform.Count<1 ||  _currentTutorialCompleted)
        {
            return;
        }
        foreach (var keyBind in _currentTutorialSceneEntry.KeyBindsNeedsToPerform)
        {
            if (Input.IsActionPressed(keyBind))
            {
                _performedKeyBinds.Add(keyBind);
            }
        }

        if(!_currentTutorialCompleted) CheckForTutorialCompletion();
    }

    private void CheckForTutorialCompletion()
    {
        foreach (var keyBind in _currentTutorialSceneEntry.KeyBindsNeedsToPerform)
        {
            if (!_performedKeyBinds.Contains(keyBind))
            {
                return;
            }
        }
        foreach (var action in _currentTutorialSceneEntry.ActionsNeedsToPerform)
        {
            if (!_performedActions.Contains(action))
            {
                //GD.Print($"{action} not performed yet");
                return;
            }
        }
        MuseumActions.TutorialSceneEntryEnded?.Invoke(_currentTutorialSceneEntry.EntryNo);
        ShowNextTutorialScene();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _httpRequestForGettingTutorial.RequestCompleted -= HttpRequestForGettingTutorialOnRequestCompleted;
        MuseumActions.OnPlayerPerformedTutorialRequiringAction -= OnPlayerPerformedTutorialRequiringAction;
        MuseumActions.PlayTutorial -= LoadTutorial;

    }
}