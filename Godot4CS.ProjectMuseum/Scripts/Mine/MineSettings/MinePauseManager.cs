using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MineSettings;

public partial class MinePauseManager : Node2D
{
    public bool IsPaused { get; set; }

    private PlayerControllerVariables _playerControllerVariables;
    
    #region Initializers

    public override void _Ready()
    {
        InitializeDiReference();
        SubscribeToActions();
    }

    private void InitializeDiReference()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }

    private void SubscribeToActions()
    {
        MineActions.OnGamePaused += Pause;
        MineActions.OnGameUnpaused += Unpause;
    }
    private void UnsubscribeToActions()
    {
        MineActions.OnGamePaused -= Pause;
        MineActions.OnGameUnpaused -= Unpause;
    }

    #endregion
    
    private void Pause()
    {
        MineActions.DeselectAllInventoryControllers?.Invoke();
        GetTree().Paused = true;
        IsPaused = true;
    }
    
    private void Unpause()
    {
        MineActions.OnToolbarSlotChanged?.Invoke(_playerControllerVariables.CurrentEquippedItemSlot);
        GetTree().Paused = false;
        IsPaused = false;
    }

    
    
    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }
}