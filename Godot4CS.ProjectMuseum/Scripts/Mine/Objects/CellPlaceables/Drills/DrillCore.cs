using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.CellPlaceables.Drills;

public partial class DrillCore : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    private MineCellCrackMaterial _mineCellCrackMaterial;

    [Export] private float _timer;
    [Export] private Label _timerLabel;

    private List<DrillDirection> _drillDirections;
    [Export] private DrillHead[] _drillHeads;
    
    [Export] private Sprite2D _drillCoreSprite2D;
    
    

    #region Initializers

    public override void _Ready()
    {
        SubscribeToActions();
        InitializeDiInstaller();
        _timer = 10f;
    }

    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _mineCellCrackMaterial = ServiceRegistry.Resolve<MineCellCrackMaterial>();
    }

    #region Subscribe and UnSubscribe

    private void SubscribeToActions()
    {
        
    }

    private void UnSubscribeToActions()
    {
        
    }

#endregion

    #endregion
    
    
    
}