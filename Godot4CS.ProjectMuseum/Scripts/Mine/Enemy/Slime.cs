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

    #region Animation Parameter Conditions

    private const string AttackCondition = "parameters/conditions/is_attacking";
    private const string MovingCondition = "parameters/conditions/is_moving";
    private const string TakingDamageCondition = "parameters/conditions/is_taking_damage";
    private const string DeathCondition = "parameters/conditions/is_dead";

    #endregion
    
    [Export] private Timer _stateChangeTimer;

    public override void _EnterTree()
    {
        TrackTimer = GetNode<Timer>("Track Timer");
        NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
    }

    public override void _Ready()
    {
        InitializeDiReferences();
        SubscribeToActions();
        Spawn();
        IsDead = false;
        Health = 100;
        StateMachine = AnimTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
    }

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        
    }

    private void OnTrackTimerTimeOut()
    {
        NavAgent.TargetPosition = IsAggro ? _playerControllerVariables.Position : MoveDirection;
    }

    private void OnStateChangeTimeOut()
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
            //GD.Print("chase method called");
            var direction = ToLocal(NavAgent.GetNextPathPosition()).Normalized();
            var directionBool = NavAgent.TargetPosition.X > Position.X;
            //_enemyAnimationController.PlayAnimation("move");
            //GD.Print($"chase velocity: {Velocity}");
            AnimationController.Sprite.FlipH = directionBool;
        
            var velocityX = direction.X * MoveSpeed;
            Velocity = new Vector2(velocityX , Velocity.Y);
            AnimTree.Set("parameters/move/blend_position", Velocity);
            StateMachine.Travel("move");
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
        AnimationController.Sprite.FlipH = directionBool;
        var velocityX = direction.X * MoveSpeed;
        Velocity = new Vector2(velocityX , Velocity.Y);
        AnimTree.Set("parameters/move/blend_position", Velocity);
        StateMachine!.Travel("move");
    }
    
    private void Idle()
    {
        Velocity = new Vector2(0, Velocity.Y);
        AnimTree.Set("parameters/move/blend_position", Velocity);
        StateMachine.Travel("move");
    }
    
    public override void Attack()
    {
        var velX = Velocity.X;
        Velocity = new Vector2(velX, Velocity.Y);
        
        if (StateMachine.GetCurrentNode() == "attack") return;
        
        AnimTree.Set(AttackCondition,true);
        AnimTree.Set(TakingDamageCondition,false);
        AnimTree.Set(DeathCondition,false);
        AnimTree.Set(MovingCondition,false);
        
        StateMachine.Travel("attack");
        
        GD.Print($"current animation: {AnimationController.CurrentAnimation}");
        _playerControllerVariables.Player.TakeDamage();
    }

    public void OnAttackAnimationFinished(string animName)
    {
        if(animName != "attack") return;
        
        AnimTree.Set(AttackCondition,false);
        AnimTree.Set(MovingCondition,true);
        AnimTree.Set(TakingDamageCondition,false);
        AnimTree.Set(DeathCondition,false);
        
        StateMachine.Travel("move");
        
        State = EnemyState.Idle;
    }

    #endregion
    
    public override void TakeDamage()
    {
        if (StateMachine.GetCurrentNode() == "damage") return;
        
        AnimTree.Set(AttackCondition,false);
        AnimTree.Set(MovingCondition,false);
        AnimTree.Set(TakingDamageCondition,true);
        
        StateMachine.Travel("damage");
        GD.Print("Enemy taking damage");
        
        Velocity = new Vector2(0, Velocity.Y);
        HealthSystem.ReduceEnemyHealth(10, 100, this);
    }
    
    private void OnTakeDamageAnimationFinished(string animName)
    {
        if(animName != "damage") return;
        
        AnimTree.Set(TakingDamageCondition,false);
        AnimTree.Set(AttackCondition,false);
        
        if (Health <= 0)
        {
            AnimTree.Set(DeathCondition,true);
            StateMachine.Travel("death");
            State = EnemyState.Death;
        }
        else
        {
            AnimTree.Set(MovingCondition,true);
            StateMachine.Travel("move");
            State = EnemyState.Move;
        }
    }

    public override void Death()
    {
        if(Health > 0) return;
        AnimTree.Set(DeathCondition,true);
        StateMachine.Travel("death");
        GD.Print("Slime Death Called");
        _ExitTree();
    }

    private void OnDeathAnimationFinished(string animeName)
    {
        if(animeName != "death") return;
        IsDead = true;
        SetPhysicsProcess(false);
        QueueFree();
    }
    
    #region Utilities

    private Vector2 GetRandomDirection()
    {
        var rand = new Random();
        return new Vector2I(rand.Next() % 2 == 0 ? 1 : -1, (int) Position.Y);
    }

    #endregion
}