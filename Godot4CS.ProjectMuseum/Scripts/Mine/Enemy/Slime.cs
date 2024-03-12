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

    #region Loitering Variables

    private Vector2 _targetPos;
    private Vector2 _leftPos;
    private Vector2 _rightPos;

    private bool _isMovingRight;
    private bool _isMovingLeft;

    [Export] private float _moveVelocity = 2;

    #endregion

    private bool _isMoving;
    
    #region Initializers

    public override void _EnterTree()
    {
        _targetPos = Vector2.Zero;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("Test"))
        {
            Phase = EnemyPhase.Teleport;
            Teleport();
        }

        if (@event.IsActionReleased("Lamp"))
            Phase = EnemyPhase.Loiter;
        if (@event.IsActionReleased("Idle"))
        {
            Phase = EnemyPhase.Loiter;
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
        StateMachine = AnimTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        IsGoingToStartingPosition = true;
        IsGoingToEndingPosition = false;
        Phase = EnemyPhase.Loiter;
        CanMove = true;
        _isMoving = true;
    }

    private void SetValuesOnSpawn()
    {
        IsDead = false;
        Health = 100;
        
        SetPhysicsProcess(false);
    }

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        OnSpawn += SetValuesOnSpawn;
        OnAttackChanged += () => Attack();
    }

    #endregion
    
    public override async void _PhysicsProcess(double delta)
    {
        Loiter();
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

    private async void Loiter()
    {
        if (_isMovingLeft)
        {
            if (_targetPos.X > Position.X)
            {
                _isMoving = false;
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
                _isMoving = false;
                _isMovingRight = false;
                _isMovingLeft = true;
                _targetPos = _leftPos;
                MoveDirection = Vector2.Left;
            }
        }

        await Move();
    }

    #endregion

    #region Teleport

    private void Teleport()
    {
        if (AnimationController.CurrentAnimation is "digIn" or "digOut") return;
        DigIn();
    }

    #endregion

    #endregion
    
    #region States

    #region Move

    private async Task Move()
    {
        if (_isMoving)
        {
            AnimationController.PlayAnimation("move");
            if (_isMovingLeft)
            {
                Velocity = new Vector2(-_moveVelocity, Velocity.Y);
                MoveAndSlide();
                AnimationController.MoveDirection(Vector2.Left);
            }
            else if (_isMovingRight)
            {
                Velocity = new Vector2(_moveVelocity, Velocity.Y);
                MoveAndSlide();
                AnimationController.MoveDirection(Vector2.Right);
            }
        }
        else
        {
            Velocity = new Vector2(0, Velocity.Y);
            MoveAndSlide();
            AnimationController.PlayAnimation("idle");
            await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
            _isMoving = true;
        }
    }

    #endregion
    
    #region Attack

    public override async Task Attack()
    {
        if(!IsAttacking) return;
        _isMoving = false;
        AnimationController.PlayAnimation("attack");
        await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
        IsAttacking = false;
        AnimationController.PlayAnimation("idle");
        await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
        _isMoving = true;
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
        public async Task Chase()
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

            await Move();
        }
    }

    #endregion

    #region Idle

    private async Task Idle()
    {
        AnimationController.PlayAnimation("idle");
    }

    private void OnIdleAnimationFinished(string animName)
    {
        if (animName != "idle") return;
        CanMove = true;
        _isMoving = true;
        //todo: check if player is in chase range. if in range then chase or else loiter
    }

    #endregion

    #endregion

    #region Physics

    #region Gravity

    [Export] private bool _isGrounded;
    private void ApplyGravity(float delta)
    {
        if(_isGrounded) return;
        var velocity = Velocity;
        velocity.X = 0f;
        velocity.Y += _gravity * delta;
        Velocity = velocity;

        MoveAndSlide();
    }
    
    private void OnCellBlockEnter(Node2D body)
    {
        var hasCollidedWithMine = body == _mineGenerationVariables.MineGenView;
        GD.Print($"enemy collided with mine: {hasCollidedWithMine})");
        if (hasCollidedWithMine)
        {
            _isGrounded = true;
            GD.Print("is Grounded");
        }
    }
    
    private void OnCellBlockExit(Node2D body)
    {
        var hasCollidedWithMine = body == _mineGenerationVariables.MineGenView;
        GD.Print($"enemy NOT collided with mine: {hasCollidedWithMine})");
        if (hasCollidedWithMine)
        {
            _isGrounded = false;
            GD.Print("is Falling");
        }
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

    private void UnsubscribeToActions()
    {
        OnSpawn -= SetValuesOnSpawn;
    }

    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }
}