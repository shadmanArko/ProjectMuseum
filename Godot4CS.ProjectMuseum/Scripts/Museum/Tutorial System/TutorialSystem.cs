using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Tutorial_System;

public partial class TutorialSystem : Node
{
    private HttpRequest _httpRequestForGettingTutorial;

    private int _currentTutorialNumber;
    private int _currentTutorialSceneNumber;

    private Tutorial _currentTutorial;
    private TutorialSceneEntry _currentTutorialSceneEntry;

    private List<string> _performedKeyBinds = new List<string>();
    private List<string> _performedActions = new List<string>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _httpRequestForGettingTutorial = new HttpRequest();
        AddChild(_httpRequestForGettingTutorial);
        _httpRequestForGettingTutorial.RequestCompleted += HttpRequestForGettingTutorialOnRequestCompleted;
        MuseumActions.OnPlayerPerformedTutorialRequiringAction += OnPlayerPerformedTutorialRequiringAction;
        ShowTutorial(2);
    }

    private void OnPlayerPerformedTutorialRequiringAction(string obj)
    {
        _performedActions.Add(obj);
        CheckForTutorialCompletion();
    }

    void ShowTutorial(int number)
    {
        GD.Print("Show tutorial called");
        
        _httpRequestForGettingTutorial.Request(ApiAddress.StoryApiPath + $"GetTutorialScene/{number}");
    }
    private void HttpRequestForGettingTutorialOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        GD.Print(jsonStr);
        _currentTutorial = JsonSerializer.Deserialize<Tutorial>(jsonStr);
        GD.Print("Show tutorial invoked");
        _currentTutorialSceneNumber = 0;
        ShowTutorialScene();
        
    }

    private void ShowTutorialScene()
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
            MuseumActions.OnTutorialEnded?.Invoke();
        }
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (_currentTutorialSceneEntry== null || _currentTutorialSceneEntry.KeyBindsNeedsToPerform.Count<1)
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

        CheckForTutorialCompletion();
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
                GD.Print($"{action} not performed yet");
                return;
            }
        }
        ShowTutorialScene();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _httpRequestForGettingTutorial.RequestCompleted -= HttpRequestForGettingTutorialOnRequestCompleted;
        MuseumActions.OnPlayerPerformedTutorialRequiringAction -= OnPlayerPerformedTutorialRequiringAction;
    }
}