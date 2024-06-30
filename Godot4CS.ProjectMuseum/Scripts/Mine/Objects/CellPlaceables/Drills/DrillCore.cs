using System;
using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.CellPlaceables.Drills;

public partial class DrillCore : Node2D, IItemizable
{
    private List<DrillDirection> _drillDirections;
    [Export] private DrillHead[] _drillHeads;
    
    [Export] private Sprite2D _drillCoreSprite2D;
    [Export] private AnimationPlayer _animationPlayer;

    public Action<DrillPhase> OnDrillPhaseChanged;
    
    #region Initializers

    public override void _Ready()
    {
        SubscribeToActions();
        InitializeDiInstaller();
    }

    private void InitializeDiInstaller()
    {
        ServiceRegistry.Resolve<PlayerControllerVariables>();
        ServiceRegistry.Resolve<MineGenerationVariables>();
        ServiceRegistry.Resolve<MineCellCrackMaterial>();
    }

    #region Subscribe and UnSubscribe

    private void SubscribeToActions()
    {
        OnDrillPhaseChanged += PlayAnimationBasedOnDrillPhase;
    }

    private void UnSubscribeToActions()
    {
        OnDrillPhaseChanged -= PlayAnimationBasedOnDrillPhase;
    }

    #endregion

    #endregion

    private void PlayAnimationBasedOnDrillPhase(DrillPhase phase)
    {
        switch (phase)
        {
            case DrillPhase.Retract:
                _animationPlayer.Play("retract");
                break;
            case DrillPhase.Expand:
                _animationPlayer.Play("expand");
                break;
            case DrillPhase.Thrust:
                _animationPlayer.Play("thrust");
                break;
            case DrillPhase.Drag:
                _animationPlayer.Play("drag");
                break;
            case DrillPhase.Disabled:
                _animationPlayer.Play("disabled");
                break;
            default:
                _animationPlayer.Play("retract");
                break;
        }
    }

    public override void _ExitTree()
    {
        UnSubscribeToActions();
    }

    public void ConvertToInventoryItem()
    {
        ReferenceStorage.Instance.InventoryItemBuilder.BuildInventoryItem("CellPlaceable", GlobalPosition);
        QueueFree();
    }

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionReleased("Test"))
            ConvertToInventoryItem();
    }
}