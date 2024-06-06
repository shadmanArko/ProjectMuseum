using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.CellPlaceables.Drills;

public partial class DrillHead : RigidBody2D
{
    private MineGenerationVariables _mineGenerationVariables;
    private MineCellCrackMaterial _mineCellCrackMaterial;
    
    [Export] private DrillCore _drillCore;
    
    [Export] private int _drillLimit = 5;
    [Export] private int _cellDamagePerHit = 5;
    
    [Export] private Sprite2D _drillExtension;
    [Export] private Sprite2D _drillHead;
    
    [Export] private DrillPhase _drillPhase;

    private DrillPhase DrillPhase
    {
        get => _drillPhase;
        set
        {
            if (_drillPhase != value)
                _drillCore.OnDrillPhaseChanged?.Invoke(value);
            _drillPhase = value;
        }
    }

    [Export] private DrillDirection _drillDirection;

    [Export] private Vector2I _initialMapPos;
    [Export] private Vector2I _finalMapPos; //to store the max position drill can reach based on its drill limit irrelevant to unbreakable or artifact cells
    [Export] private Vector2I _targetMapPos; //to store the furthest position the drill can reach to dig a wall within the _finalMapPos 

    [Export] private Vector2 _initialGlobalPos; //to store global initial and final positions of drill
    [Export] private Vector2 _targetGlobalPos;
    
    [Export] private Vector2 _initialWreckPos;  //to store wrecker's thrusting starting and ending positions
    [Export] private Vector2 _targetWreckPos;
    
    [Export] private int _wreckRetractPosOffset;   //30
    [Export] private int _wreckThrustPosOffset;   //10
    
    [Export] private int _thrustSpeed;  //300 force at which the weight moves to target wreck position
    [Export] private int _withdrawSpeed;    //20 force at which the weight goes back to initial wreck position

    [Export] private float _speed;
    [Export] private float _stoppingDistance;
    
    public override async void _Ready()
    {
        InitializeDiInstaller();
        SubscribeToActions();
        SetPhysicsProcess(false);
        await Task.Delay(5000);
        InitializeDrill();
    }

    private void InitializeDiInstaller()
    {
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _mineCellCrackMaterial = ServiceRegistry.Resolve<MineCellCrackMaterial>();
    }

    private void SubscribeToActions()
    {
        
    }

    private void UnsubscribeToActions()
    {
        
    }

    private float _timer;

    public override async void _PhysicsProcess(double delta)
    {
        SetExtensionScale();
        
        if (DrillPhase == DrillPhase.Expand)
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
            }
        }
        else if (DrillPhase == DrillPhase.Thrust)
        {
            var playOnlyOnce = false;
            if (GlobalPosition.DistanceTo(_targetWreckPos) > _stoppingDistance)
            {
                var direction = (_targetWreckPos - GlobalPosition).Normalized(); 
                LinearVelocity = direction * _thrustSpeed;
            }
            else
            {
                DrillPhase = DrillPhase.Drag;
                LinearVelocity = Vector2.Zero;

                if (!playOnlyOnce)
                {
                    BreakCell(_targetMapPos + GetDrillDirection());
                    var drillDirection = GetDrillDirection();
                    var shakeDirection = drillDirection.X != 0 ? 
                        ShakeDirection.Horizontal : ShakeDirection.Vertical;
                    ReferenceStorage.Instance.ScreenShakeController.ShakeScreen(ShakeIntensity.Moderate, shakeDirection);
                    playOnlyOnce = true;
                }
                await Task.Delay(2000);
                if (!ContainsCellsToBreak())
                {
                    DrillPhase = DrillPhase.Retract;
                    RetractToCore();
                }
            }
        }
        else if (DrillPhase == DrillPhase.Drag)
        {
            if (GlobalPosition.DistanceTo(_initialWreckPos) > _stoppingDistance)
            {
                var direction = (_initialWreckPos - GlobalPosition).Normalized(); 
                LinearVelocity = direction * _withdrawSpeed;
            }
            else
            {
                var cellToBreak = GetCellByMapPos(_targetMapPos + GetDrillDirection());
                if (cellToBreak.HitPoint <= 0)
                {
                    LinearVelocity = Vector2.Zero;
                    await Task.Delay(2000);
                    DrillPhase = DrillPhase.Retract;
                    RetractToCore();
                }
                else 
                    DrillPhase = DrillPhase.Thrust;
            }
        }
        else if (DrillPhase == DrillPhase.Retract)
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

                if (!ContainsCellsToBreak())
                {
                    _drillPhase = DrillPhase.Disabled;
                }
                else
                {
                    DrillPhase = DrillPhase.Expand;
                    InitializeDrill();
                }
            }
        }
    }

    private void InitializeDrill()
    {
        CalculateInitialAndFinalPosition();
        var posToExpandTo = _initialMapPos;

        if (!ContainsCellsToBreak())
            _drillPhase = DrillPhase.Disabled;
        
        for (var i = 1; i <= _drillLimit; i++)
        {
            var cellPos = _initialMapPos + GetDrillDirection() * i;
            var cell = _mineGenerationVariables.GetCell(cellPos);
            if (cell == null)
            {
                GD.PrintErr("WARNING! CELL NOT FOUND");
                return;
            }
            
            if (!cell.IsBreakable || !cell.IsInstantiated)
            {
                GD.PrintErr("CELL NOT BREAKABLE OR NOT INITIALIZED");
                break;
            }
            
            if (cell.IsBroken) posToExpandTo = new Vector2I(cell.PositionX, cell.PositionY);
            else break;
        }
        
        if (posToExpandTo == _initialMapPos || posToExpandTo == _finalMapPos)
        {
            DrillPhase = DrillPhase.Disabled;
        }
        else
        {
            _targetMapPos = posToExpandTo;
            ExpandUptoFurthestEmptyCell();
            DrillPhase = DrillPhase.Expand;
        }
    }

    private void ExpandUptoFurthestEmptyCell()
    {
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cellOffset = new Vector2(cellSize / 2f, cellSize / 4f);
        _initialGlobalPos = _initialMapPos * cellSize + cellOffset;
        _targetGlobalPos = _targetMapPos * cellSize + cellOffset;
        SetPhysicsProcess(true);
    }

    private void RetractToCore()
    {
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cellOffset = new Vector2(cellSize, cellSize) / 2;
        
        var targetCellPos = GetLocalToMap(_drillCore.GlobalPosition);
        var targetCell = GetCellByMapPos(targetCellPos);
        _targetGlobalPos = new Vector2(targetCell.PositionX, targetCell.PositionY) * cellSize + cellOffset;
    }
    
    private void WreckCellWalls()
    {
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var currentGlobalPosition = GetLocalToMap(GlobalPosition) * cellSize + new Vector2(cellSize, cellSize)/2;
        _initialWreckPos = currentGlobalPosition + new Vector2(0, -_wreckRetractPosOffset);
        _targetWreckPos = currentGlobalPosition + new Vector2(0, _wreckThrustPosOffset);
        DrillPhase = DrillPhase.Thrust;
    }
    
    private void BreakCell(Vector2I tilePos)
    {
        var cell = _mineGenerationVariables.GetCell(tilePos);
        cell.HitPoint -= _cellDamagePerHit;
        Math.Clamp(-_mineGenerationVariables.GetCell(tilePos).HitPoint, 0, 100000);

        var normalCellCrackMaterial =
            _mineCellCrackMaterial!.CellCrackMaterials.FirstOrDefault(cellCrackMat =>
                cellCrackMat.MaterialType == "Normal");
        MineSetCellConditions.SetCrackOnTiles(tilePos, GetDrillDirection(), cell,
            normalCellCrackMaterial);
        if (cell.HitPoint <= 0)
        {
            var cells = MineCellDestroyer.DestroyCellByPosition(tilePos, _mineGenerationVariables);
            var caveCells = CaveControlManager.RevealCave(_mineGenerationVariables, cells);

            foreach (var tempCell in cells)
            {
                var tempCellPos = new Vector2I(tempCell.PositionX, tempCell.PositionY);
                var cellCrackMaterial =
                    _mineCellCrackMaterial.CellCrackMaterials[0];
                MineSetCellConditions.SetTileMapCell(tempCellPos, GetDrillDirection(), tempCell,
                    cellCrackMaterial, _mineGenerationVariables);
            }
            
            foreach (var tempCell in caveCells)
            {
                var tempCellPos = new Vector2I(tempCell.PositionX, tempCell.PositionY);
                var cellCrackMaterial =
                    _mineCellCrackMaterial.CellCrackMaterials[0];
                MineSetCellConditions.SetTileMapCell(tempCellPos, GetDrillDirection(), tempCell,
                    cellCrackMaterial, _mineGenerationVariables);
            }

            _mineGenerationVariables.BrokenCells++;
            MineActions.OnMineCellBroken?.Invoke(tilePos);
        }
    }

    #region Utilities

    private bool ContainsCellsToBreak()
    {
        var unbrokenCellCount = 0;
        for (int i = 1; i <= _drillLimit; i++)
        {
            var cell = GetCellByMapPos(_initialMapPos + GetDrillDirection() * i);
            if(cell == null) continue;
            if(!cell.IsInstantiated || !cell.IsBreakable) break;
            
            if (!cell.IsBroken)
                unbrokenCellCount++;
        }
        
        return unbrokenCellCount > 0;
    }
    
    private void CalculateInitialAndFinalPosition()
    {
        _initialMapPos = GetLocalToMap(_drillCore.GlobalPosition);
        _finalMapPos = _initialMapPos + GetDrillDirection() * _drillLimit;
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

    private void SetExtensionScale()
    {
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var difference = (_drillHead.GlobalPosition.Y - _drillExtension.GlobalPosition.Y) / cellSize;
        var extensionScaleY = Mathf.Clamp(difference, 0.5f,1000);
        _drillExtension.GlobalScale = new Vector2(_drillExtension.GlobalScale.X, extensionScaleY);
    }

    #endregion

    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }
}