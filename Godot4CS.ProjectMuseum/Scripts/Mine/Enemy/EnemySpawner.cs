using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class EnemySpawner : Node2D
{
    private List<Enemy> _enemies;
    
    [Export] private Node _parentNode;
    [Export] private string _slimePrefabPath;

    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    
    [Export] private Vector2 _p0;
    [Export] private Vector2 _p1;
    [Export] private Vector2 _p2;
    
    [Export] private double _time;
    
    private Vector2 _newPos = new(560,-60);

    [Export] private int[] _cellBreakTargetCount;
    [Export] private int[] _enemySpawnCount;
    [Export] private int _counter;
    [Export] private bool _enemyMovingIntoMine;

    public override void _EnterTree()
    {
        InitializeDiInstallers();
    }

    public override void _Ready()
    {
        _enemies = new List<Enemy>();
        SetProcess(false);
        SetPhysicsProcess(false);
        _counter = 0;
    }

    private void InitializeDiInstallers()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void OnTimeEndSpawnEnemy()
    {
        var enemiesAlive = 0;
        foreach (var tempEnemy in _enemies)
        {
            if (!tempEnemy.IsDead)
                enemiesAlive++;
        }
        
        if(enemiesAlive >= 3) return;
        CheckForEnemySpawnConditions();
    }

    private void CheckForEnemySpawnConditions()
    {
        if(_counter >= _enemySpawnCount.Length) return;

        if (_enemySpawnCount[_counter] <= 0)
            _counter++;

        if (_mineGenerationVariables.BrokenCells >= _cellBreakTargetCount[_counter])
        {
            SpawnEnemy();
            GD.Print($"SPAWNING SLIME OF COUNTER {_counter} AFTER BREAKING {_mineGenerationVariables.BrokenCells} CELLS (TARGET:{_cellBreakTargetCount[_counter]})");
            _enemySpawnCount[_counter]--;
        }
    }

    private void SpawnEnemy()
    {
        var scene = ResourceLoader.Load<PackedScene>(_slimePrefabPath).Instantiate();
        GD.Print($"Slime Scene is null {scene is null}");
        _parentNode.AddChild(scene);
        var enemy = scene as Enemy;
        if (enemy == null)
        {
            GD.PrintErr("Enemy is null");
            return;
        }

        GD.Print("Spawning Enemy");
        _newEnemy = enemy;
        enemy.Position = new Vector2(730, -58);
        enemy.SetProcess(false);
        SetProcess(true);
    }

    private Enemy _newEnemy;
    
    public override void _Process(double delta)
    {
        AutoMoveToPosition(_newEnemy);
    }
	
    public override void _PhysicsProcess(double delta)
    {
        _newEnemy.Position = AutoJumpIntoMine((float) _time);
        _time += delta;
        
        if(_newEnemy.Velocity.Y > 0)
            _newEnemy.AnimationController.Play("fall");
        if (!(_time >= 1.2f)) return;
        _newEnemy.AnimationController.Play("idle");
        SetProcess(false);
        SetPhysicsProcess(false);
        if(!_enemies.Contains(_newEnemy))
            _enemies.Add(_newEnemy);
        _newEnemy.IsAffectedByGravity = true;
        _newEnemy.OnSpawn?.Invoke();
        _newEnemy.Phase = EnemyPhase.Loiter;
        _newEnemy = null;
    }
    
    #region Auto Animations
    
    private void AutoMoveToPosition(Enemy enemy)
    {
        if(enemy == null) return;
        if(enemy.Position.X > _newPos.X)
        {
            enemy.Translate(new Vector2(-0.2f,0));
            enemy.AnimationController.Play("move");
        }
        else
        {
            var targetCell = _mineGenerationVariables.GetCell(new Vector2I(24, 0));
            _p0 = enemy.Position;
            _p2 = enemy.Position + new Vector2(targetCell.PositionX,targetCell.PositionY) * _mineGenerationVariables.Mine.CellSize;
            _p1 = new Vector2((_p0.X + _p2.X) / 2, _p0.Y - 75);
            
            SetPhysicsProcess(true);
        }
    }
    private Vector2 AutoJumpIntoMine(float t)
    {
        var q0 = _p0.Lerp(_p1, t);
        var q1 = _p1.Lerp(_p2, t);
        var r = q0.Lerp(q1, t);
        return r;
    }
    
    #endregion
}