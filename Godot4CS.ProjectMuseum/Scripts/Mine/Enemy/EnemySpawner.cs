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
    [Export] private bool _canSpawn;
    
    private Vector2 _newPos = new(560,-60);

    private List<int> _cellBreakTargetCount;
    private List<int> _enemySpawnCount;
    [Export] private int _counter;
    [Export] private bool _enemyMovingIntoMine;
    
    #region Initializers

    public override void _EnterTree()
    {
        
    }
    
    public override void _Ready()
    {
        InitializeDiInstallers();
        _enemies = new List<Enemy>();
        _cellBreakTargetCount = new List<int> { 10, 20, 30, 40, 50, 60 };
        _enemySpawnCount = new List<int> { 1,1,2,1,2,3};
        SetProcess(false);
        SetPhysicsProcess(false);
        _counter = 0;
        _canSpawn = true;
    }

    private void InitializeDiInstallers()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    #endregion

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
        if(_counter >= _enemySpawnCount.Count) return;

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
        // GD.Print($"Slime Scene is null {scene is null}");
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
        // enemy.SetProcess(false);
        // SetProcess(true);
        _canSpawn = false;
    }

    private Enemy _newEnemy;
    
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
            var offset = new Vector2(_mineGenerationVariables.Mine.CellSize / 2f,0);
            _p0 = enemy.Position;
            _p2 = new Vector2(targetCell.PositionX, targetCell.PositionY) * _mineGenerationVariables.Mine.CellSize + offset;
            _p1 = new Vector2(_p2.X, _p0.Y - 20);
            _time = 0;
            
            SetProcess(false);
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
    
    #region Processes

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
        _newEnemy.Phase = EnemyPhase.Explore;
        _newEnemy = null;
    }

    #endregion
    
    [Export] private bool _isGrounded;
    private void ApplyGravity()
    {
        if (_isGrounded)
        {
            _newEnemy.Velocity = new Vector2(_newEnemy.Velocity.X, 0);
            return;
        }
        _newEnemy.Velocity = new Vector2(0, _newEnemy.Velocity.Y + 20);
        _newEnemy.MoveAndSlide();
    }
}