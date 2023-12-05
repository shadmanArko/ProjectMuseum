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
    
    // [Export] private float _moveSpeed = 20;
    
    // public string Id { get; set; }
    // [Export] public NavigationAgent2D NavAgent { get; set; }
    // [Export] public Timer TrackTimer { get; set; }
    // public EnemyState State { get; set; }
    // public bool IsAffectedByGravity { get; set; }

    // [Export] private bool _isAggro;
    private Vector2 _moveDirection;
    // [Export] private float _aggroRange = 140f;

    // private int _health;
    // public int Health
    // {
    //     get => _health;
    //     set
    //     {
    //         _health = value;
    //         _healthBar.Value = _health;
    //         
    //         if(_health <= 0)
    //             Die();
    //     }
    // }

    //[Export] private TextureProgressBar _healthBar;
    
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

    private void OnTrackTimerTimeOut()
    {
        NavAgent.TargetPosition = IsAggro ? _playerControllerVariables.Position : _moveDirection;
    }

    public void OnStateChangeTimeOut()
    {
        _moveDirection = GetRandomDirection();
        
        if (State == EnemyState.Idle)
            State = IsAggro ? EnemyState.Chase : EnemyState.Move;
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
        //ApplyGravity((float) delta);
        
        if (State == EnemyState.Move)
            Move();
        // else if(State == EnemyState.Chase)
        //     Chase();
        // else if(State == EnemyState.Idle)
        //     Idle();
        // else if (State == EnemyState.Attack)
        //     Attack();
        else if (State == EnemyState.DigIn)
            DigIn();

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
        //GD.Print($"distance is :{distance}");
        if (distance > AggroRange) return;

        if (IsAggro) return;
        //_enemyAnimationController.Play("aggro");
        IsAggro = true;
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
        
        Velocity = Vector2.Zero;
        NavAgent.TargetPosition = Position;
        SetPhysicsProcess(false);
        Visible = false;
    }
    

    public override void Chase()
    {
        GD.Print("chase method called");
        var direction = ToLocal(NavAgent.GetNextPathPosition()).Normalized();
        var directionBool = NavAgent.TargetPosition.X > Position.X;
        _enemyAnimationController.Play("move");
        _enemyAnimationController.Sprite.FlipH = directionBool;
        
        var velocityX = direction.X * MoveSpeed;
        Velocity = new Vector2(velocityX , Velocity.Y);
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
        
        var velocityX = direction.X * MoveSpeed;
        Velocity = new Vector2(velocityX , Velocity.Y);
        //MoveAndSlide();
    }

    

    private void Idle()
    {
        Velocity = Vector2.Zero;
        _enemyAnimationController.Play("idle");
    }
    
    public override void Attack()
    {
        State = EnemyState.Attack;
        Velocity = Vector2.Zero;
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

    public override void TakeDamage()
    {
        GD.Print("Enemy taking damage");
        _enemyAnimationController.Play("damage");
        HealthSystem.ReduceEnemyHealth(10, 100, this);
    }

    public void Die()
    {
        _enemyAnimationController.Play("death");
        SetPhysicsProcess(false);
        _ExitTree();
    }

    public void OnDeathAnimationFinished(string animeName)
    {
        if(animeName != "death") return;
        QueueFree();
    }
}