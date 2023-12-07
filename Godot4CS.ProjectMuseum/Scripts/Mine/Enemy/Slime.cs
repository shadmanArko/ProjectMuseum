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
    //[Export] private EnemyAnimationController _enemyAnimationController;
    
    [Export] private AnimationTree _animationTree;
    private AnimationNodeStateMachinePlayback _stateMachine;
    
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
        _stateMachine = _animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
    }

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        HealthBar.Changed += Die;
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
        AnimationController.PlayAnimation("aggro");
        IsAggro = true;
    }

    #region Slime Possible States
    
    private void Spawn()
    {
        State = EnemyState.DigOut;
        var tilePos = _mineGenerationVariables.GetCell(new Vector2I(24, 0));
        Position = new Vector2(tilePos.PositionX * 20, tilePos.PositionY);
        Visible = true;
        AnimationController.PlayAnimation("digOut");
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
        AnimationController.PlayAnimation("digOut");
    }

    public void OnDigOutAnimationFinished(string animName)
    {
        if(animName != "digOut") return;
        State = EnemyState.Idle;
    }
    
    private void DigIn()
    {
        AnimationController.PlayAnimation("digIn");
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
            //_enemyAnimationController.PlayAnimation("move");
            GD.Print($"chase velocity: {Velocity}");
            AnimationController.Sprite.FlipH = directionBool;
        
            var velocityX = direction.X * MoveSpeed;
            Velocity = new Vector2(velocityX , Velocity.Y);
            _animationTree.Set("parameters/move/blend_position", Velocity);
            _stateMachine.Travel("move");
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
        // _enemyAnimationController.PlayAnimation("move");
        AnimationController.Sprite.FlipH = directionBool;
        var velocityX = direction.X * MoveSpeed;
        Velocity = new Vector2(velocityX , Velocity.Y);
        _animationTree.Set("parameters/move/blend_position", Velocity);
        GD.Print($"_stateMachine is null: {_stateMachine == null}");
        _stateMachine!.Travel("move");
    }
    
    private void Idle()
    {
        Velocity = new Vector2(0, Velocity.Y);
        GD.Print($"idle velocity {Velocity}");
        _animationTree.Set("parameters/move/blend_position", Velocity);
        _stateMachine.Travel("move");
    }
    
    public override void Attack()
    {
        var velX = Velocity.X;
        Velocity = new Vector2(velX, Velocity.Y);
        GD.Print("ENEMY ATTACKING");
        if (_stateMachine.GetCurrentNode() == "attack")
        {
            GD.Print("Current animation is ATTACK");
            return;
        }
        
        
        _animationTree.Set("parameters/conditions/is_moving",false);
        _animationTree.Set("parameters/conditions/is_attacking",true);
        _stateMachine.Travel("attack");
        GD.Print($"current animation: {AnimationController.CurrentAnimation}");
        _playerControllerVariables.Player.TakeDamage();
    }

    public void OnAttackAnimationFinished(string animName)
    {
        if(animName != "attack") return;
        _animationTree.Set("parameters/conditions/is_attacking",false);
        _animationTree.Set("parameters/conditions/is_moving",true);
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
        AnimationController.PlayAnimation("damage");
        HealthSystem.ReduceEnemyHealth(10, 100, this);
    }

    private void Die()
    {
        if(Health > 0) return;
        AnimationController.PlayAnimation("death");
        SetPhysicsProcess(false);
        _ExitTree();
    }

    public void OnDeathAnimationFinished(string animeName)
    {
        if(animeName != "death") return;
        QueueFree();
    }
}