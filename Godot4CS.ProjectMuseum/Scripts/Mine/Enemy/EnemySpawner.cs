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
    
    private bool _autoMoveToPos;
    private bool _jumpIntoMine;
    
    private Vector2 _newPos = new(560,-60);

    public override void _EnterTree()
    {
        InitializeDiInstallers();
    }

    public override void _Ready()
    {
        _enemies = new List<Enemy>();
        SetProcess(false);
        SetPhysicsProcess(false);
    }

    private void InitializeDiInstallers()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void OnTimeEndSpawnEnemy()
    {
        //GD.Print("Spawning new enemy");
        if(_enemies.Count >= 3) return;
        var scene = ResourceLoader.Load<PackedScene>(_slimePrefabPath).Instantiate();
        GD.Print($"Slime Scene is null {scene is null}");
        _parentNode.AddChild(scene);
        var enemy = scene as Enemy;
        if (enemy == null)
        {
            GD.PrintErr("Enemy is null");
            return;
        }

        _newEnemy = enemy;
        _autoMoveToPos = false;
        _jumpIntoMine = true;
        enemy.Position = new Vector2(730, -58);
        
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
        _newEnemy.State = EnemyState.Move;
        _newEnemy = null;
    }
    
    #region Auto Animations
    
    private void AutoMoveToPosition(Enemy enemy)
    {
        if(enemy == null) return;
        if(enemy.Position.X > _newPos.X)
        {
            enemy.Translate(new Vector2(-0.02f,0));
            enemy.AnimationController.Play("move");
        }
        else
        {
            _p0 = enemy.Position;
            _p2 = enemy.Position + new Vector2(-60, 0);
            _p1 = new Vector2((_p0.X + _p2.X) / 2, _p0.Y - 75);
            _autoMoveToPos = true;
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

    
    #region For Testing

    public override void _Input(InputEvent @event)
    {
        // if (@event.IsActionReleased("Enemy"))
        // {
        //     OnTimeEndSpawnEnemy();
        // }
    }

    #endregion
}