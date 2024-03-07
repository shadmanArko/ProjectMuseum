using System;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Slime : Enemy
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    #region Animation Parameter Conditions

    private const string AttackCondition = "parameters/conditions/is_attacking";
    private const string MovingCondition = "parameters/conditions/is_moving";
    private const string TakingDamageCondition = "parameters/conditions/is_taking_damage";
    private const string DeathCondition = "parameters/conditions/is_dead";

    #endregion

    private EnemyAi _enemyAi = new();
    [Export] private Timer _stateChangeTimer;

    [Export] private bool _isTest;

    #region Random Loitering

    private Vector2 _targetPos;
    private Vector2 _leftPos;
    private Vector2 _rightPos;

    private bool _isMovingRight;
    private bool _isMovingLeft;

    [Export] private float _moveVelocity = 2;

    #endregion
    

    #region Initializers

    public override void _EnterTree()
    {
        TrackTimer = GetNode<Timer>("Track Timer");
        NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _targetPos = Vector2.Zero;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("Test"))
        {
            State = EnemyState.Teleport;
            Teleport();
        }
        if (@event.IsActionReleased("Lamp"))
            State = EnemyState.Loiter;
        if (@event.IsActionReleased("Idle"))
        {
            State = EnemyState.Idle;
            AnimationController.PlayAnimation("idle");
        }
        if(@event.IsActionReleased("randomMovement"))
            DecideMoveTargetPosition();
    }

    public override void _Ready()
    {
        InitializeDiReferences();
        SubscribeToActions();
        _enemyAi = new EnemyAi();
        IsDead = false;
        Health = 100;
        StateMachine = AnimTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        IsGoingToStartingPosition = true;
        IsGoingToEndingPosition = false;
        State = EnemyState.Idle;
        CanMove = true;
    }

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        
    }

    #endregion


    public override void _PhysicsProcess(double delta)
    {
        if(IsAggro)
            Chase();
        switch (State)
        {
            case EnemyState.Loiter:
                Loiter();
                break;
        }
    }

    #region Phases

    #region Loiter

    private void DecideMoveTargetPosition()
    {
        var tuple = _enemyAi.HorizontalMovement(Position);
        if(tuple == null) return;
        _leftPos = tuple.Item1;
        _rightPos = tuple.Item2;

        _targetPos = _leftPos;
        MoveDirection = Vector2.Left;
        _isMovingLeft = true;
        _isMovingRight = false;
    }
    
    private void Loiter()
    {
        if (_isMovingLeft)
        {
            if (_targetPos.X > Position.X)
            {
                Velocity = new Vector2(0, Velocity.Y);
                _isMovingLeft = false;
                _isMovingRight = true;
                _targetPos = _rightPos;
                MoveDirection = Vector2.Right;
            }
        }

        if (_isMovingRight)
        {
            if (_targetPos.X < Position.X)
            {
                Velocity = new Vector2(0, Velocity.Y);
                _isMovingRight = false;
                _isMovingLeft = true;
                _targetPos = _leftPos;
                MoveDirection = Vector2.Left;
            }
        }
        
        Move();
    }

    #endregion
    
    private void Teleport()
    {
        DigIn();
    }

    #endregion

    private void Move()
    {
        AnimationController.PlayAnimation("move");
        if (_isMovingLeft)
        {
            Velocity = new Vector2(-_moveVelocity, Velocity.Y);
            AnimationController.MoveDirection(Vector2.Left);
        }
        else if (_isMovingRight)
        {
            Velocity = new Vector2(_moveVelocity, Velocity.Y);
            AnimationController.MoveDirection(Vector2.Right);
        }
        
        MoveAndSlide();
    }

    #region Dig In Dig Out

    private void DigIn()
    {
        AnimationController.PlayAnimation("digIn");
    }

    private void OnDigInAnimationFinished(string animName)
    {
        if(animName != "digIn") return;
        DigOut();
    }
    
    private void DigOut()
    {
        var currentEnemyPos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
        var digOutPos = _enemyAi.SlimeDigOut(currentEnemyPos);
        if(digOutPos.Equals(Vector2.Zero)) return;
        Position = digOutPos;
        AnimationController.PlayAnimation("digOut");
    }

    private void OnDigOutAnimationFinished(string animName)
    {
        if(animName != "digOut") return;
        State = EnemyState.Loiter;
    }

    #endregion

    #region Chase

    override 
    public void Chase()
    {
        var posToGo = _enemyAi.ChasePlayer(Position);
        if (posToGo == Vector2.Zero)
            DigIn();
        else
        {
            _targetPos = posToGo;
        }
    }

    #endregion

    #region Idle

    private void Idle()
    {
        AnimationController.PlayAnimation("idle");
    }

    private void OnIdleAnimationFinished(string animName)
    {
        if(animName != "idle") return;
        CanMove = true;
        //todo: check if player is in chase range. if in range then chase or else loiter
    }

    #endregion
    
    private void KnockBack()
    {
        var playerDirection = _playerControllerVariables.PlayerDirection;
        var knockBackDirection = (playerDirection - Velocity).Normalized() * KnockBackPower;
        Velocity = knockBackDirection;
        MoveAndSlide();
    }
}