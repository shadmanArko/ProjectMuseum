using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.CellPlaceables.Drills;

public partial class DrillHead : RigidBody2D
{
    private MineGenerationVariables _mineGenerationVariables;
    
    [Export] private DrillCore _drillCore;
    
    [Export] private int _drillLimit = 5;
    
    [Export] private Sprite2D _drillExtension;
    [Export] private Sprite2D _drillHead;
    
    [Export] private DrillPhase _drillPhase;
    [Export] private DrillDirection _drillDirection;

    [Export] private Vector2I _initialMapPos;
    [Export] private Vector2I _finalMapPos; //to store the max position drill can reach based on its drill limit irrelevant to unbreakable or artifact cells
    [Export] private Vector2I _targetMapPos; //to store the furthest position the drill can reach to dig a wall within the _finalMapPos 

    [Export] private Vector2 _initialGlobalPos; //to store global initial and final positions of drill
    [Export] private Vector2 _targetGlobalPos;
    
    [Export] private Vector2 _initialWreckPos;  //to store wrecker's thrusting starting and ending positions
    [Export] private Vector2 _targetWreckPos;
    [Export] private bool _isWrecking;
    [Export] private int _wreckPosOffset;   //30
    
    [Export] private int _thrustSpeed;  //300 force at which the weight moves to target wreck position
    [Export] private int _withdrawSpeed;    //20 force at which the weight goes back to initial wreck position

    [Export] private float _speed;
    [Export] private float _stoppingDistance;
    [Export] private int _drillCount;
    
    public override void _EnterTree()
    {
        
    }

    public override async void _Ready()
    {
        InitializeDiInstaller();
        SubscribeToActions();
        SetPhysicsProcess(false);
        await InitializeDrill();
       GD.PrintErr("INSIDE READY");
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

    public override async void _PhysicsProcess(double delta)
    {
        if (_drillPhase == DrillPhase.Expand)
        {
            if (GlobalPosition.DistanceTo(_targetGlobalPos) > _stoppingDistance)
            {
                var direction = (_targetGlobalPos - GlobalPosition).Normalized(); 
                LinearVelocity = direction * _speed;
            }
            else
            {
                LinearVelocity = Vector2.Zero;
                WreckCellWalls();
                await Task.Delay(1000);
                _drillPhase = DrillPhase.Drill;
                GD.Print("Expand to wreck walls");
            }
        }
        else if (_drillPhase == DrillPhase.Drill)
        {
            if (_isWrecking)
            {
                if (GlobalPosition.DistanceTo(_targetWreckPos) > _stoppingDistance)
                {
                    var direction = (_targetWreckPos - GlobalPosition).Normalized(); 
                    LinearVelocity = direction * _thrustSpeed;
                }
                else
                {
                    _isWrecking = false;
                    LinearVelocity = Vector2.Zero;
                    await Task.Delay(2000);
                    _drillCount++;
                }
            }
            else
            {
                if (GlobalPosition.DistanceTo(_initialWreckPos) > _stoppingDistance)
                {
                    var direction = (_initialWreckPos - GlobalPosition).Normalized(); 
                    LinearVelocity = direction * _withdrawSpeed;
                }
                else
                {
                    if (_drillCount >= 5)
                    {
                        LinearVelocity = Vector2.Zero;
                        _isWrecking = false;
                        await Task.Delay(2000);
                        _drillCount = 0;
                        _drillPhase = DrillPhase.Retract;
                        RetractToCore();
                    }
                    else
                    {
                        _isWrecking = true;
                    }
                    
                }
            }
        }
        else if (_drillPhase == DrillPhase.Retract)
        {
            if (GlobalPosition.DistanceTo(_targetGlobalPos) > _stoppingDistance)
            {
                var direction = (_targetGlobalPos - GlobalPosition).Normalized(); 
                LinearVelocity = direction * _speed;
            }
            else
            {
                LinearVelocity = Vector2.Zero;
                await Task.Delay(2000);
                _drillPhase = DrillPhase.Expand;
                ExpandUptoFurthestEmptyCell();
            }
        }
    }

    private async Task InitializeDrill()
    {
        await Task.Delay(5000);
        CalculateInitialAndFinalPosition();
        var posToExpandTo = _initialMapPos;
        
        for (var i = 0; i < _drillLimit; i++)
        {
            var cellPos = _initialMapPos + GetDrillDirection() * i;
            var cell = _mineGenerationVariables.GetCell(cellPos);
            if (cell == null)
            {
                GD.PrintErr("WARNING! CELL NOT FOUND");
                return;
            }
            
            GD.PrintErr("FOUND VALID CELL");
            // if (!cell.IsBreakable || !cell.IsInstantiated)
            // {
            //     GD.PrintErr("CELL NOT BREAKABLE OR NOT INITIALIZED");
            //     break;
            // }
            if (cell.IsBroken) posToExpandTo = new Vector2I(cell.PositionX, cell.PositionY);
        }

        if (posToExpandTo == _initialMapPos)
        {
            _drillPhase = DrillPhase.Disabled;
            GD.PrintErr("Disabled drill as next pos equal to initial pos");
        }
        else
        {
            _targetMapPos = posToExpandTo;
            ExpandUptoFurthestEmptyCell();
            _drillPhase = DrillPhase.Expand;
            GD.PrintErr("EXPANDING DRILL HEAD");
        }
    }

    private void ExpandUptoFurthestEmptyCell()
    {
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cellOffset = new Vector2(cellSize, cellSize) / 2;
        _initialGlobalPos = _initialMapPos * cellSize + cellOffset;
        _targetGlobalPos = _targetMapPos * cellSize + cellOffset;
        SetPhysicsProcess(true);
        GD.Print("initial pos and final pos calculated. expanding");
    }

    private void RetractToCore()
    {
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cellOffset = new Vector2(cellSize, cellSize) / 2;
        
        var targetCellPos = GetLocalToMap(_drillCore.GlobalPosition);
        var targetCell = GetCellByMapPos(targetCellPos);
        _targetGlobalPos = new Vector2(targetCell.PositionX, targetCell.PositionY) * cellSize + cellOffset;
        GD.Print("retracting to core");
        // _targetGlobalPos = new Vector2(0, 25);
    }
    
    private void WreckCellWalls()
    {
        _initialWreckPos = GlobalPosition + new Vector2(0, -_wreckPosOffset);
        _targetWreckPos = GlobalPosition + new Vector2(0, _wreckPosOffset);
        _isWrecking = true;
        GD.Print("wrecking cell walls");
    }

    #region Utilities

    private void CalculateInitialAndFinalPosition()
    {
        
        GD.Print($"drill core position: {_drillCore.GlobalPosition}");
        _initialMapPos = GetLocalToMap(_drillCore.GlobalPosition);
        _finalMapPos = _initialMapPos + GetDrillDirection() * _drillLimit;
        GD.PrintErr("CALCULATING INITIAL AND FINAL MAP POSITION");
    }

    private Vector2I GetDrillDirection()
    {
        var direction = _drillDirection switch
        {
            DrillDirection.Up => Vector2I.Up,
            DrillDirection.Down => Vector2I.Down,
            DrillDirection.Left => Vector2I.Left,
            DrillDirection.Right => Vector2I.Right,
            DrillDirection.Disabled => Vector2I.Zero,
            _ => Vector2I.Zero
        };

        return direction;
    }

    private Vector2I GetLocalToMap(Vector2 globalPos)
    {
        var mapPos = _mineGenerationVariables.MineGenView.LocalToMap(globalPos);
        return mapPos;
    }

    private Cell GetCellByMapPos(Vector2I mapPos)
    {
        var cell = _mineGenerationVariables.GetCell(mapPos);
        if (cell == null)
        {
            GD.PrintErr("Fatal Error: Cell not found");
            return null;
        }

        return cell;
    }

    #endregion

    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }
}