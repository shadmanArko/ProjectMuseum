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

    private bool IsMovingRight;
    private bool IsMovingLeft;

    [Export] private float _moveVelocity = 2;

    #endregion

    
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
        CanMove = true;
        IsMoving = true;
        SetPhysicsProcess(true);
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
    }

    #endregion

    #region Move Into Mine

    private async Task MoveIntoTheMine()
    {
        if (Position.X <= _targetPos.X + 20 && Position.Y >= 0)
        {
            _isInsideMine = true;
            OnSpawn?.Invoke();
            return;
        }
        var cell = _mineGenerationVariables.GetCell(new Vector2I(24, 0));
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        _targetPos = new Vector2(cell.PositionX, cell.PositionY) * cellSize;
        IsMovingLeft = true;
        IsMovingRight = false;
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
                // GD.Print($"isAttacking: {IsAttacking}");
                // GD.Print($"Phase: {Phase}");
                // GD.Print();
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
        IsMovingLeft = true;
        IsMovingRight = false;
    }

    private async Task Loiter()
    {
        if (_targetPos == Vector2.Zero)
            DecideMoveTargetPosition();
        
        if (IsMovingLeft)
        {
            if (_targetPos.X > Position.X)
            {
                IsMoving = false;
                IsMovingLeft = false;
                IsMovingRight = true;
                _targetPos = _rightPos;
                MoveDirection = Vector2.Right;
            }
        }

        if (IsMovingRight)
        {
            if (_targetPos.X < Position.X)
            {
                IsMoving = false;
                IsMovingRight = false;
                IsMovingLeft = true;
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
                IsMovingLeft = true;
                IsMovingRight = false;
                MoveDirection = Vector2.Left;
            }
            else if (_targetPos.X > Position.X)
            {
                IsMovingLeft = false;
                IsMovingRight = true;
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
        if (IsMoving && !IsAttacking)
        {
            if(IsInAttackRange) return;
            AnimationController.PlayAnimation("move");
            if (IsMovingLeft)
            {
                Velocity = new Vector2(-_moveVelocity, Velocity.Y);
                MoveAndSlide();
                AnimationController.MoveDirection(Vector2.Left);
            }
            else if (IsMovingRight)
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
            AnimationController.Play("idle");
            await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
            IsMoving = true;
        }
    }

    #endregion
    
    #region Attack

    public override async Task Attack()
    {
        if(!IsInAttackRange) return;
        if(!IsAttacking) return;
        IsMoving = false;
        IsAttacking = false;
        GD.Print("Inside attack method");
        var lookAtPlayer = new Vector2(_playerControllerVariables.Position.X - Position.X, 0).Normalized();
        AnimationController.MoveDirection(lookAtPlayer);
        AnimationController.PlayAnimation("attack");
        //TODO: player taking damage must come from damage database where the damage value and status effects are registered
        MineActions.OnTakeDamageStarted?.Invoke(10);
        await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
        GD.Print("attack animation complete");
        AnimationController.PlayAnimation("idle");
        await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
        IsMoving = true;
    }

    #endregion

    #region Take Damage

    public override void TakeDamage()
    {
        if (IsTakingDamage) return;
        IsMoving = false;
        
        AnimationController.PlayAnimation("damage");
        HealthSystem.ReduceEnemyHealth(10, 100, this);
        Velocity = new Vector2(0, Velocity.Y);
        KnockBack();
        IsMoving = true;
    }

    #endregion

    #region Death

    public override async void Death()
    {
        AnimationController.PlayAnimation("death");
        await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength * 1000));
        // QueueFree();
        GD.Print("ENEMY DYING");
    }

    private void OnDeathAnimationComplete(string animName)
    {
        if(animName != "death") return;
        QueueFree();
    }

    #endregion

    #region Dig In Dig Out

    private void DigIn()
    {
        IsMoving = false;
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
        IsMoving = true;
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
        IsMoving = true;
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
        var knockBackVel = Mathf.Lerp(Velocity.X,knockBackDirection.X, 0.1f);
        Velocity = Vector2.Zero;
        Velocity = new Vector2(knockBackVel,Velocity.Y);
        GD.Print($"getting knocked back {knockBackVel}");
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