using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces.Movement;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Slime : CharacterBody2D, IUnit, IMovement, IAttack
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    [Export] private EnemyAnimationController _enemyAnimationController;
    [Export] private Timer _stateChangeTimer;
    
    [Export] private float _moveSpeed = 20;
    
    public string Id { get; set; }
    [Export] public NavigationAgent2D NavAgent { get; set; }
    [Export] public Timer TrackTimer { get; set; }
    public EnemyState State { get; set; }
    public bool IsAffectedByGravity { get; set; }

    [Export] private bool _isAggro;
    private Vector2 _moveDirection;
    [Export] private float _aggroRange = 140f;
    
    public override void _EnterTree()
    {
        InitializeDiReferences();
    }

    public override void _Ready()
    {
       Spawn();
    }

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void OnTrackTimerTimeOut()
    {
        NavAgent.TargetPosition = _isAggro ? _playerControllerVariables.Position : _moveDirection;
    }

    public void OnStateChangeTimeOut()
    {
        _moveDirection = GetRandomDirection();
        
        if (State == EnemyState.Idle)
            State = _isAggro ? EnemyState.Chase : EnemyState.Move;
        else if (State is EnemyState.Move or EnemyState.Chase)
            State = EnemyState.DigIn;
        else if (State == EnemyState.DigIn)
        {
            State = EnemyState.DigOut;
            DigOut();
        }
        else if (State == EnemyState.DigOut)
            State = EnemyState.Idle;
        GD.Print($"State changed to {State}");
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckPlayerAggro();
        
        if (State == EnemyState.Move)
            Move();
        else if(State == EnemyState.Chase)
            Chase();
        else if(State == EnemyState.Idle)
            Idle();
        else if (State == EnemyState.Attack)
            Attack();
        else if (State == EnemyState.DigIn)
            DigIn();
        
        ApplyGravity((float) delta);
    }


    [Export] private float _gravity;
    private void ApplyGravity(float delta)
    {
        var collision = MoveAndCollide(Velocity, recoveryAsCollision: true);
        
        if (collision == null)
        {
            var velocity = Velocity;
            velocity.Y += _gravity * delta;
            Velocity = velocity;
        }
    }

    private void CheckPlayerAggro()
    {
        var currentPos = new System.Numerics.Vector2(Position.X, Position.Y);
        var playerPos = new System.Numerics.Vector2(_playerControllerVariables.Position.X, _playerControllerVariables.Position.Y);
        var distance = System.Numerics.Vector2.Distance(currentPos, playerPos);
        if (distance > _aggroRange) return;

        if (_isAggro) return;
        _enemyAnimationController.Play("aggro");
        _isAggro = true;
    }

    #region Slime Possible States
    
    private void Spawn()
    {
        State = EnemyState.DigOut;
        var tilePos = _mineGenerationVariables.GetCell(new Vector2I(24, 0));
        Position = new Vector2(tilePos.PositionX * 20, tilePos.PositionY);
        Visible = true;
        _enemyAnimationController.Play("digOut");
        SetPhysicsProcess(true);
    }

    private void DigOut()
    {
        SetPhysicsProcess(true);
        var digOutPos =EnemyDigOutEmptyCellChecker.CheckForEmptyCellsAroundPlayer(_playerControllerVariables.Position, _mineGenerationVariables);
        if (digOutPos != Vector2.Zero)
        {
            Position = digOutPos;
        }
        else
        {
            Position = Position;
        }
        Visible = true;
        _enemyAnimationController.Play("digOut");
    }

    public void OnDigOutAnimationFinished(string animName)
    {
        if(animName != "digOut") return;
        State = EnemyState.Idle;
    }
    
    private void DigIn()
    {
        _enemyAnimationController.Play("digIn");
    }
    
    public void OnDigInAnimationFinished(string animName)
    {
        if(animName != "digIn") return;
        
        NavAgent.TargetPosition = Position;
        Velocity = Vector2.Zero;
        SetPhysicsProcess(false);
        Visible = false;
    }
    

    public void Chase()
    {
        var direction = ToLocal(NavAgent.GetNextPathPosition()).Normalized();
        var directionBool = NavAgent.TargetPosition.X > Position.X;
        _enemyAnimationController.Play("move");
        _enemyAnimationController.Sprite.FlipH = directionBool;
        
        Velocity = direction * _moveSpeed * Vector2.Right;
        MoveAndCollide(Velocity);
    }
    
    public void OnAggroAnimationFinished(string animName)
    {
        if(animName != "aggro") return;
        State = EnemyState.Move;
    }

    public void Move()
    {
        var direction = ToLocal(NavAgent.GetNextPathPosition()).Normalized();
        var directionBool = NavAgent.TargetPosition.X > Position.X;
        _enemyAnimationController.Play("move");
        _enemyAnimationController.Sprite.FlipH = directionBool;
        
        Velocity = direction * _moveSpeed * Vector2.Right;
        Velocity.MoveToward(direction, 0.2f);
        MoveAndCollide(Velocity);
    }

    

    private void Idle()
    {
        _enemyAnimationController.Play("idle");
    }
    
    public void Attack()
    {
        State = EnemyState.Attack;
        _enemyAnimationController.Play("attack");
    }

    public void OnAttackAnimationFinished(string animName)
    {
        if(animName != "attack") return;
        State = EnemyState.Idle;
    }
    
    public void OnTakeDamageAnimationFinished(string animName)
    {
        if(animName != "damage") return;
        
    }

    #endregion

    #region Utilities

    private Vector2 GetRandomDirection()
    {
        var rand = new Random();
        return new Vector2I(rand.Next() % 2 == 0 ? 1 : -1, (int) Position.Y);
    }

    #endregion
    
}