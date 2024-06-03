using System;
using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.CellPlaceables.Drills;

public partial class DrillCore : Node2D
{
    public Action<DrillPhase> OnDrillPhaseChanged;
    
    private List<DrillDirection> _drillDirections;
    [Export] private DrillHead[] _drillHeads;
    
    [Export] private Sprite2D _drillCoreSprite2D;
    [Export] public AnimationPlayer _animationPlayer;

    [Export] private Texture2D _disabledCoreTexture;
    [Export] private Texture2D _enabledCoreTexture;
    
    #region Initializers

    public override void _Ready()
    {
        SubscribeToActions();
        InitializeDiInstaller();
    }

    private void InitializeDiInstaller()
    {
        
    }

    #region Subscribe and UnSubscribe

    private void SubscribeToActions()
    {
        OnDrillPhaseChanged += PlayAnimationOnDrillPhaseChanged;
    }

    private void UnSubscribeToActions()
    {
        OnDrillPhaseChanged -= PlayAnimationOnDrillPhaseChanged;
    }

    #endregion

    #endregion

    public void DisableCore()
    {
        // _drillCoreSprite2D.Texture = _disabledCoreTexture;
    }

    public void EnableCore()
    {
        // _drillCoreSprite2D.Texture = _enabledCoreTexture;
    }

    private void PlayAnimationOnDrillPhaseChanged(DrillPhase phase)
    {
        switch (phase)
        {
            case DrillPhase.Retract:
                _animationPlayer.Play("drag");
                break;
            case DrillPhase.Expand:
                _animationPlayer.Play("expand");
                break;
            case DrillPhase.Drill:
                _animationPlayer.Play("thrust");
                break;
            case DrillPhase.Disabled:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
        }
    }
    
}