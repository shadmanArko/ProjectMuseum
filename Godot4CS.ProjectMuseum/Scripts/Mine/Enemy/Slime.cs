using System.Threading.Tasks;
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

    #region Animation Parameter Conditions

    private const string AttackCondition = "parameters/conditions/is_attacking";
    private const string MovingCondition = "parameters/conditions/is_moving";
    private const string TakingDamageCondition = "parameters/conditions/is_taking_damage";
    private const string DeathCondition = "parameters/conditions/is_dead";

    #endregion

    private EnemyAi _enemyAi = new();

    #region Loitering Variables

    [Export] private Vector2 _targetPos;
    private Vector2 _leftPos;
    private Vector2 _rightPos;

    private bool _isMovingRight;
    private bool _isMovingLeft;

    [Export] private float _moveVelocity = 2;

    #endregion

    [Export] private bool _isMoving;
    [Export] private bool _isInsideMine;
    
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
        Phase = EnemyPhase.Teleport;
        // CanMove = false;
        // _isMoving = false;
        CanMove = true;
        _isMoving = true;
        SetPhysicsProcess(true);
        // SetPhysicsProcess(false);
    }

    private void SetValuesOnSpawn()
    {
        IsDead = false;
        Health = 100;
        DecideMoveTargetPosition();
        CanMove = true;
        SetPhysicsProcess(true);
    }

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        OnSpawn += SetValuesOnSpawn;
        // OnAttackChanged += () => Attack();
    }

    #endregion

    #region Move Into Mine

    private async Task MoveIntoTheMine()
    {
        if (Position.X <= _targetPos.X + 20 && Position.Y >= 0)
        {
            _isInsideMine = true;
            DecideMoveTargetPosition();
            return;
        }
        var cell = _mineGenerationVariables.GetCell(new Vector2I(24, 0));
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        _targetPos = new Vector2(cell.PositionX, cell.PositionY) * cellSize;
        _isMovingLeft = true;
        _isMovingRight = false;
        await Move();
    }

    #endregion

    #region Start Slime Activity

    private async Task StartSlimeActivity()
    {
        if (IsAggro)
        {
            if (Phase != EnemyPhase.Hurt)
            {
                if (Phase != EnemyPhase.Combat)
                {
                    var validPos = _enemyAi.CheckForPathValidity(Position);
                    Phase = validPos != Vector2.Zero ? EnemyPhase.Chase : EnemyPhase.Teleport;
                }
            }
        }
        else
        {
            Phase = EnemyPhase.Loiter;
        }

        switch (Phase)
        {
            case EnemyPhase.Loiter:
                await Loiter();
                break;
            case EnemyPhase.Chase:
                await Chase();
                break;
            case EnemyPhase.Teleport:
                Teleport();
                break;
            case EnemyPhase.Combat:
                GD.Print($"isAttacking: {IsAttacking}");
                GD.Print($"Phase: {Phase}");
                GD.Print();
                await Attack();
                break;
        }
    }

    #endregion
    
    public override async void _PhysicsProcess(double delta)
    {
        if (!_isInsideMine)
            await MoveIntoTheMine();
        else
            await StartSlimeActivity();
        
        ApplyGravity();
    }

    #region Phases

    #region Loiter

    private void DecideMoveTargetPosition()
    {
        var tuple = _enemyAi.DetermineLoiteringPath(Position);
        GD.Print($"tuple is null: {tuple == null}");
        if (tuple == null)
        {
            IsAggro = true;
            Phase = EnemyPhase.Teleport;
            return;
        }
        _leftPos = tuple.Item1;
        _rightPos = tuple.Item2;
        GD.Print($"leftPos: {_leftPos}, rightPos: {_rightPos}");

        _targetPos = _leftPos;
        MoveDirection = Vector2.Left;
        _isMovingLeft = true;
        _isMovingRight = false;
    }

    private async Task Loiter()
    {
        if (_targetPos == Vector2.Zero)
            DecideMoveTargetPosition();
        
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
    
    #region Chase

    public override async Task Chase()
    {
        var posToGo = _enemyAi.CheckForPathValidity(Position);
        var currentAnim = AnimationController.CurrentAnimation;
        if (posToGo == Vector2.Zero && (currentAnim != "digIn" || currentAnim != "digOut" || currentAnim != "idle"))
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
    
    #endregion
    
    #region States

    #region Move

    private async Task Move()
    {
        if (_isMoving && !IsAttacking)
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
            Idle();
            await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
            _isMoving = true;
        }
    }

    #endregion
    
    #region Attack

    public override async Task Attack()
    {
        if(!IsInAttackRange) return;
        if(!IsAttacking) return;
        _isMoving = false;
        IsAttacking = false;
        GD.Print("Inside attack method");
        var lookAtPlayer = new Vector2(_playerControllerVariables.Position.X - Position.X, 0).Normalized();
        AnimationController.MoveDirection(lookAtPlayer);
        AnimationController.PlayAnimation("attack");
        _playerControllerVariables.Player.TakeDamage();
        await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
        GD.Print("attack animation complete");
        AnimationController.PlayAnimation("idle");
        await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
        _isMoving = true;
    }

    #endregion

    #region Take Damage

    public override void TakeDamage()
    {
        if (IsTakingDamage) return;
        _isMoving = false;
        
        AnimationController.PlayAnimation("damage");
        HealthSystem.ReduceEnemyHealth(10, 100, this);
        Velocity = new Vector2(0, Velocity.Y);
        MoveAndSlide();
        KnockBack();
        _isMoving = true;
    }

    #endregion

    #region Death

    public override async void Death()
    {
        AnimationController.PlayAnimation("death");
        await Task.Delay((int)AnimationController.CurrentAnimationLength * 1000);
        QueueFree();
        GD.Print("ENEMY DYING");
    }

    #endregion

    #region Dig In Dig Out

    private void DigIn()
    {
        _isMoving = false;
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
        DecideMoveTargetPosition();
    }

    private void OnDigOutAnimationFinished(string animName)
    {
        if (animName != "digOut") return;
        _isMoving = true;
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
        _isMoving = true;
    }

    #endregion

    #endregion

    #region Physics
    
    #region Gravity

    [Export] private bool _isGrounded;
    private void ApplyGravity()
    {
        if (_isGrounded)
        {
            Velocity = new Vector2(Velocity.X, 0);
            return;
        }
        Velocity = new Vector2(0, Velocity.Y + _gravity);
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
        // GD.Print($"enemy NOT collided with mine: {hasCollidedWithMine})");
        if (hasCollidedWithMine)
        {
            _isGrounded = false;
            // GD.Print("is Falling");
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
        Velocity = Velocity.Lerp(knockBackDirection, 0.2f);
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
        SetPhysicsProcess(false);
        UnsubscribeToActions();
    }
}