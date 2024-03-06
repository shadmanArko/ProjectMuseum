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
        switch (State)
        {
            case EnemyState.Loiter:
                Loiter();
                break;
        }
    }

    #region Phases

    private void DecideMoveTargetPosition()
    {
        var tuple = _enemyAi.HorizontalMovement(Position);
        if(tuple == null) return;
        _leftPos = tuple.Item1;
        _rightPos = tuple.Item2;

        _targetPos = (_targetPos.X - Position.X) switch
        {
            < 0 => _rightPos,
            > 0 => _leftPos,
            _ => _targetPos
        };

        if (_targetPos == _leftPos)
            MoveDirection = _leftPos;
        else if (_targetPos == _rightPos)
            MoveDirection = _rightPos;
    }
    
    private void Loiter()
    {
        // For Random Loitering
        // if (_targetPos == Vector2.Zero || _targetPos.X - Position.X < 0 && MoveDirection == Vector2.Left ||
        //     _targetPos.X - Position.X > 0 && MoveDirection == Vector2.Right)
        // {
        //     _targetPos = _enemyAi.HorizontalMovement(Position);
        //     GD.Print($"target pos: {_targetPos.X},{_targetPos.Y}");
        //     MoveDirection = (_targetPos.X - Position.X) switch
        //     {
        //         < 0 => Vector2.Left,
        //         > 0 => Vector2.Right,
        //         _ => MoveDirection
        //     };
        // }

        

        if (_targetPos == _rightPos)
            MoveDirection = Vector2.Right;
        else if(_targetPos == _leftPos)
            MoveDirection = Vector2.Left;
            
        Move();
    }
    
    private void Teleport()
    {
        DigIn();
    }

    #endregion

    private void Move()
    {
        GD.Print($"leftPos:{_leftPos.X}, rightPos:{_rightPos.X}, pos:{Position.X}");
        if(_targetPos.X - Position.X < 0)
            DecideMoveTargetPosition();
        Velocity = new Vector2(MoveDirection == Vector2.Left ? -_moveVelocity : _moveVelocity, 0);
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
        // AnimationController.PlayAnimation("idle");
        State = EnemyState.Loiter;
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