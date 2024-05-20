using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.CellPlaceables.Drills;

public partial class DrillHead : Node2D
{
    private MineGenerationVariables _mineGenerationVariables;
    
    [Export] private DrillCore _drillCore;

    [Export] private Timer _timer;
    [Export] private int _drillLimit = 5;
    
    [Export] private Sprite2D _drillExtension;
    [Export] private Sprite2D _drillHead;
    
    [Export] private DrillPhase _drillPhase;
    [Export] private DrillDirection _drillDirection;

    [Export] private Vector2I _initialMapPos;
    [Export] private Vector2I _finalMapPos; //to store the max position drill can reach based on its drill limit irrelevant to unbreakable or artifact cells
    [Export] private Vector2I _targetMapPos; //to store the furthest position the drill can reach to dig a wall within the _finalMapPos 

    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        InitializeDiInstaller();
        SubscribeToActions();
        _drillPhase = DrillPhase.Disabled;
        _drillDirection = DrillDirection.Down;
    }

    private void InitializeDiInstaller()
    {
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        
    }

    private void UnsubscribeToActions()
    {
        
    }
    
    public override void _Process(double delta)
    {
        // if(_initialMapPos.Y < _targetMapPos.Y)
            
            
    }

    private void InitializeDrill()
    {
        CalculateInitialAndFinalPosition();
        var posToExpandTo = _initialMapPos;

        for (int i = _initialMapPos.Y; i < _finalMapPos.Y; i++)
        {
            var cellPos = new Vector2I(_finalMapPos.X, i);
            var cell = _mineGenerationVariables.GetCell(cellPos);
            if (cell == null)
            {
                GD.PrintErr("WARNING! CELL NOT FOUND");
                return;
            }
            
            if(!cell.IsBreakable || !cell.IsInstantiated) break;
            if (cell.IsBroken) posToExpandTo = new Vector2I(cell.PositionX, cell.PositionY);
        }

        if (posToExpandTo == _initialMapPos)
        {
            _drillPhase = DrillPhase.Disabled;
        }
        else
        {
            _drillPhase = DrillPhase.Expand;
            _targetMapPos = posToExpandTo;
            ExpandUptoFurthestEmptyCell();
        }
    }

    private async void ExpandUptoFurthestEmptyCell()
    {
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cellOffset = new Vector2(cellSize, cellSize) / 2;
        var initialGlobalPos = _initialMapPos * cellSize + cellOffset;
        var targetGlobalPos = _targetMapPos * cellSize + cellOffset;

        
    }

    private void RetractToCore()
    {
        
    }

    private void DrillCellWall()
    {
        
    }

    #region Utilities

    private void CalculateInitialAndFinalPosition()
    {
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cellOffset = new Vector2I(cellSize, cellSize) /2;
        var initialPos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
        // var targetPos = _drillDirection switch
        // {
        //     DrillDirection.Left => initialPos + new Vector2I(-_drillLimit, 0),
        //     DrillDirection.Right => initialPos + new Vector2I(_drillLimit, 0),
        //     DrillDirection.Up => initialPos + new Vector2I(0, -_drillLimit),
        //     DrillDirection.Down => initialPos + new Vector2I(0, _drillLimit),
        //     DrillDirection.Disabled => initialPos,
        //     _ => _targetGlobalPos + new Vector2I(-_drillLimit, 0)
        // };

        _initialMapPos = initialPos;
        _finalMapPos = initialPos + new Vector2I(0, _drillLimit);
    }

    #endregion

    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }
}