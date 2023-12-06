using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Slime : Enemy
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    [Export] private EnemyAnimationController _enemyAnimationController;
    [Export] private Timer _stateChangeTimer;

    public override void _EnterTree()
    {
        InitializeDiReferences();
        TrackTimer = GetNode<Timer>("Track Timer");
        NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
    }

    public override void _Ready()
    {
        Spawn();
        Health = 100;
    }

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        _healthBar.Changed += Die;
    }

    private void OnTrackTimerTimeOut()
    {
        NavAgent.TargetPosition = IsAggro ? _playerControllerVariables.Position : MoveDirection;
    }

    public void OnStateChangeTimeOut()
    {
        MoveDirection = GetRandomDirection();

        State = State switch
        {
            EnemyState.Idle => IsAggro ? EnemyState.Chase : EnemyState.Move,
            EnemyState.Move or EnemyState.Chase => EnemyState.Idle,
            _ => State
        };

        GD.Print($"State changed to {State}");
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckPlayerAggro();

        switch (State)
        {
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }

        var collisionBool = MoveAndSlide();
        if(IsAffectedByGravity) ApplyGravity(collisionBool, (float) delta);
    }


    [Export] private float _gravity;
    private void ApplyGravity(bool collisionBool, float delta)
    {
        if (!collisionBool)
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
        if (distance > AggroRange) return;

        if (IsAggro) return;
        _enemyAnimationController.PlayAnimation("aggro");
        IsAggro = true;
    }

    #region Slime Possible States
    
    private void Spawn()
    {
        State = EnemyState.DigOut;
        var tilePos = _mineGenerationVariables.GetCell(new Vector2I(24, 0));
        Position = new Vector2(tilePos.PositionX * 20, tilePos.PositionY);
        Visible = true;
        _enemyAnimationController.PlayAnimation("digOut");
        SetPhysicsProcess(true);
    }

    private void DigOut()
    {
        SetPhysicsProcess(true);
        var digOutPos = EnemyDigOutEmptyCellChecker.CheckForEmptyCellsAroundPlayer(_playerControllerVariables.Position, _mineGenerationVariables);
        if (digOutPos != Vector2.Zero)
        {
            Position = digOutPos;
        }
        else
        {
            Position = Position;
        }
        Visible = true;
        _enemyAnimationController.PlayAnimation("digOut");
    }

    public void OnDigOutAnimationFinished(string animName)
    {
        if(animName != "digOut") return;
        State = EnemyState.Idle;
    }
    
    private void DigIn()
    {
        _enemyAnimationController.PlayAnimation("digIn");
    }
    
    public void OnDigInAnimationFinished(string animName)
    {
        if(animName != "digIn") return;
        
        Velocity = Vector2.Zero;
        NavAgent.TargetPosition = Position;
        SetPhysicsProcess(false);
        Visible = false;
    }
    

    public override void Chase()
    {
        if (NavAgent.IsTargetReached())
        {
            State = EnemyState.Attack;
        }
        else
        {
            GD.Print("chase method called");
            var direction = ToLocal(NavAgent.GetNextPathPosition()).Normalized();
            var directionBool = NavAgent.TargetPosition.X > Position.X;
            _enemyAnimationController.PlayAnimation("move");
            _enemyAnimationController.Sprite.FlipH = directionBool;
        
            var velocityX = direction.X * MoveSpeed;
            Velocity = new Vector2(velocityX , Velocity.Y);
        }
    }
    
    public void OnAggroAnimationFinished(string animName)
    {
        if(animName != "aggro") return;
        State = EnemyState.Move;
    }

    public void Move()
    {
        NavAgent.TargetPosition = MoveDirection;
        var direction = ToLocal(NavAgent.GetNextPathPosition()).Normalized();
        var directionBool = NavAgent.TargetPosition.X > Position.X;
        _enemyAnimationController.PlayAnimation("move");
        _enemyAnimationController.Sprite.FlipH = directionBool;
        var velocityX = direction.X * MoveSpeed;
        Velocity = new Vector2(velocityX , Velocity.Y);
    }
    
    private void Idle()
    {
        Velocity = Vector2.Zero;
        _enemyAnimationController.PlayAnimation("idle");
    }
    
    public override void Attack()
    {
        Velocity = Vector2.Zero;
        if(_enemyAnimationController.CurrentAnimation == "attack") return;
        _enemyAnimationController.PlayAnimation("attack");
        _playerControllerVariables.Player.TakeDamage();
    }

    public void OnAttackAnimationFinished(string animName)
    {
        if(animName != "attack") return;
        State = EnemyState.Idle;
    }
    
    public void OnTakeDamageAnimationFinished(string animName)
    {
        if(animName != "damage") return;
        State = EnemyState.Move;

    }

    #endregion

    #region Utilities

    private Vector2 GetRandomDirection()
    {
        var rand = new Random();
        return new Vector2I(rand.Next() % 2 == 0 ? 1 : -1, (int) Position.Y);
    }

    #endregion

    public override void TakeDamage()
    {
        GD.Print("Enemy taking damage");
        _enemyAnimationController.PlayAnimation("damage");
        HealthSystem.ReduceEnemyHealth(10, 100, this);
    }

    private void Die()
    {
        if(Health > 0) return;
        _enemyAnimationController.PlayAnimation("death");
        SetPhysicsProcess(false);
        _ExitTree();
    }

    public void OnDeathAnimationFinished(string animeName)
    {
        if(animeName != "death") return;
        QueueFree();
    }
}