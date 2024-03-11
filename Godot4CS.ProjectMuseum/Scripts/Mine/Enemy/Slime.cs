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
    // [Export] private Timer _stateChangeTimer;

    // [Export] private bool _isTest;

    #region Loitering Variables

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
            SetPhysicsProcess(true);
        }

        if (@event.IsActionReleased("randomMovement"))
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
        SetPhysicsProcess(false);
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
        if (IsAggro)
        {
            Chase();
        }
        else
        {
            Loiter();
        }
    }

    #region Phases

    #region Loiter

    private void DecideMoveTargetPosition()
    {
        var tuple = _enemyAi.DetermineLoiteringPath(Position);
        if (tuple == null) return;
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
        if (AnimationController.CurrentAnimation is "digIn" or "digOut") return;
        DigIn();
    }

    #endregion
    
    #region States

    #region Move

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

    #endregion
    
    #region Attack

    public override void Attack()
    {
        AnimationController.PlayAnimation("attack");
    }

    #endregion

    #region Dig In Dig Out

    private void DigIn()
    {
        AnimationController.PlayAnimation("digIn");
    }

    private void OnDigInAnimationFinished(string animName)
    {
        if (animName != "digIn") return;
        DigOut();
    }

    private void DigOut()
    {
        var currentEnemyPos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
        var digOutPos = _enemyAi.DetermineDigOutPosition(currentEnemyPos);
        if (digOutPos.Equals(Vector2.Zero)) return;
        Position = digOutPos;
        AnimationController.PlayAnimation("digOut");
    }

    private void OnDigOutAnimationFinished(string animName)
    {
        if (animName != "digOut") return;
    }

    #endregion

    #region Chase

    override
        public void Chase()
    {
        var posToGo = _enemyAi.CheckForPathValidity(Position);
        if (posToGo == Vector2.Zero)
        {
            Teleport();
        }
        else
        {
            _targetPos = posToGo;
            if (_targetPos.X < Position.X)
            {
                _isMovingLeft = true;
                _isMovingRight = false;
                MoveDirection = Vector2.Left;
            }
            else if (_targetPos.X > Position.X)
            {
                _isMovingLeft = false;
                _isMovingRight = true;
                MoveDirection = Vector2.Right;
            }

            Move();
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
        if (animName != "idle") return;
        CanMove = true;
        //todo: check if player is in chase range. if in range then chase or else loiter
    }

    #endregion

    #endregion

    #region Physics

    #region Gravity

    private void ApplyGravity()
    {
        
    }

    #endregion

    #region Knock Back

    private void KnockBack()
    {
        var playerDirection = _playerControllerVariables.PlayerDirection;
        var knockBackDirection = (playerDirection - Velocity).Normalized() * KnockBackPower;
        Velocity = knockBackDirection;
        MoveAndSlide();
    }

    #endregion

    #endregion

    #region Utitlies

    #region Chase Range

    private void OnPlayerEnterChaseRange(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        IsAggro = true;
        GD.Print($"Player entered chase region, isAggro:{IsAggro}");
    }

    private void OnPlayerExitChaseRange(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        IsAggro = false;
        GD.Print($"Player exited chase region, isAggro:{IsAggro}");
    }

    #endregion
    
    #region Attack Range

    private void OnPlayerEnterAttackRange(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        IsAttacking = true;
        GD.Print($"Player entered ATTACK region, isAttacking:{IsAttacking}");
    }

    private void OnPlayerExitAttackRange(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        IsAttacking = false;
        GD.Print($"Player exited ATTACK region, isAttacking:{IsAttacking}");
    }

    #endregion
    
    #endregion
}