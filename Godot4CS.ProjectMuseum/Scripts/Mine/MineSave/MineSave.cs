using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.DTOs;
using Time = ProjectMuseum.Models.Time;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MineSave;

public partial class MineSave : Node
{
    private PlayerControllerVariables _playerControllerVariables;
    private InventoryDTO _inventoryDto;

    public override void _Ready()
    {
        SubscribeToActions();
        InitializeDiReference();
    }

    private void SubscribeToActions()
    {
        MineActions.OnMineGameEnd += SaveGame;
    }

    private void InitializeDiReference()
    {
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }

    private void SaveGame()
    {
        var saveData = SaveLoadService.Load();
        
        GD.Print($"save file location: {DataPath.SaveDataFolderPath}");
        AddMineArtifactsToSaveArtifactStorage(saveData.ArtifactStorage);
        saveData.Inventory = _inventoryDto.Inventory;
        saveData.PlayerInfo = _playerControllerVariables.PlayerInfo;
        SaveTime(saveData.Time);
        SaveLoadService.Save(saveData);
    }

    #region Save Time

    private void SaveTime(Time time)
    {
        time.Days++;
        var daysInMuseum = time.Days;
        var daysInMine = ReferenceStorage.Instance.MineTimeSystem.GetTime().Days;
        var totalTimePassed = daysInMuseum + daysInMine;
        time.Days = totalTimePassed;
    }

    #endregion

    #region Saving Artifacts to Artifact Storage

    private void AddMineArtifactsToSaveArtifactStorage(global::ProjectMuseum.Models.Artifact_and_Inventory.ArtifactStorage saveArtifactStorage)
    {
        var artifactStorage = _inventoryDto.ArtifactStorage;
        foreach (var artifact in artifactStorage.Artifacts)
        {
            saveArtifactStorage.Artifacts.Add(artifact);
            GD.Print($"Storing artifact: {artifact.RawArtifactId}");
        }
    }

    #endregion

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("randomMovement"))
        {
            GD.Print("SAVE Game pressed");
            SaveGame();
        }
    }

    #region Finalizers

    private void UnsubscribeToActions()
    {
        MineActions.OnMineGameEnd -= SaveGame;
    }

    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }

    #endregion
}