using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Mine.WallPlaceables;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Slime : Enemy
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    private EnemyAi _enemyAi = new();

    #region Loitering Variables

    [Export] private Vector2 _targetPos;
    private Vector2 _leftPos;
    private Vector2 _rightPos;

    [Export] private bool _isMovingRight;
    [Export] private bool _isMovingLeft;

    [Export] private float _moveVelocity = 2;

    #endregion

    #region Exploring Variables

    [Export] private bool _hasWallOnLeft;
    [Export] private bool _hasWallOnRight;

    #endregion

    [Export] private bool _isInsideMine;

    #region Initializers

    public override void _EnterTree()
    {
        _targetPos = Vector2.Zero;
    }

    public override void _Ready()
    {
        InitializeDiReferences();
        SubscribeToActions();
        _enemyAi = new EnemyAi();
        IsGoingToStartingPosition = true;
        IsGoingToEndingPosition = false;
        Phase = EnemyPhase.Explore;
        IsDead = false;
        CanMove = true;
        IsMoving = true;
    }

    private void SetValuesOnSpawn()
    {
        IsDead = false;
        Health = 25;
        IsAggro = false;
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
        OnAggroChanged += PlayAggroAnimation;
        OnTakeDamage += EnemyDamageAnimation;
    }

    #endregion

    #region Move Into Mine

    private async void MoveIntoTheMine()
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
        _isMovingLeft = true;
        _isMovingRight = false;
        await Move();
    }

    #endregion

    #region Start Slime Activity

    private async void StartSlimeActivity()
    {
        if(!IsAggro) Phase = EnemyPhase.Explore;

        switch (Phase)
        {
            case EnemyPhase.Explore:
                await Explore();
                break;
            case EnemyPhase.Chase:
                await Chase();
                break;
            case EnemyPhase.Combat:
                Attack();
                break;
        }
    }

    #endregion

    public override void _PhysicsProcess(double delta)
    {
        if (IsDead)
        {
            SetPhysicsProcess(false);
            return;
        }

        if (!_isInsideMine)
            MoveIntoTheMine();
        else
            StartSlimeActivity();

        ApplyGravity();
    }

    #region Phases
    
    #region Explore

    private async Task Explore()
    {
        if (_hasWallOnLeft && _hasWallOnRight)
        {
            AnimationController.PlayAnimation("idle");
            await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
        }

        if (_hasWallOnLeft)
        {
            _isMovingLeft = false;
            _isMovingRight = true;
        }
        else if (_hasWallOnRight)
        {
            _isMovingLeft = true;
            _isMovingRight = false;
        }

        await Move();
    }

    #endregion

    // #region Teleport
    //
    // private bool _isTeleporting;
    // private async Task Teleport()
    // {
    //     if(_isTeleporting) return;
    //     GD.Print("INSIDE TELEPORT METHOD");
    //     _isTeleporting = true;
    //     IsMoving = false;
    //     
    //     var currentEnemyPos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
    //     var digOutPos = _enemyAi.DetermineDigOutPosition(currentEnemyPos);
    //     if (digOutPos == Vector2.Zero)
    //     {
    //         AnimationController.PlayAnimation("idle");
    //         await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
    //         IsMoving = true;
    //         _isTeleporting = false;
    //         return;
    //     }
    //     
    //     if(AnimationController.CurrentAnimation != "")
    //         await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
    //     AnimationController.PlayAnimation("idle");
    //     await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
    //     AnimationController.PlayAnimation("digIn");
    //     await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
    //     TurnOffEnemyVisibility();
    //     
    //     await Task.Delay(3000);
    //     Position = digOutPos;
    //     TurnOnEnemyVisibility();
    //     
    //     AnimationController.PlayAnimation("digOut");
    //     await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
    //     AnimationController.PlayAnimation("idle");
    //     await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
    //
    //     if (_random.Next(1, 9999) % 2 == 0)
    //     {
    //         _isMovingLeft = true;
    //         _isMovingRight = false;
    //     }
    //     else
    //     {
    //         _isMovingLeft = false;
    //         _isMovingRight = true;
    //     }
    //     
    //     IsMoving = true;
    //     _isTeleporting = false;
    // }
    //
    // #endregion

    #region Chase

    public override async Task Chase()
    {
        if (!IsAggro)
        {
            AnimationController.PlayAnimation("aggro");
            await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
        }

        var posToGo = _enemyAi.CheckForPathValidity(Position);
        if (posToGo == Vector2.Zero)
        {
            AnimationController.PlayAnimation("idle");
            await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
            Phase = EnemyPhase.Explore;
            return;
        }

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

    #endregion

    #endregion

    #region States

    #region Move

    private async Task Move()
    {
        if (IsMoving && !IsAttacking)
        {
            if (IsInAttackRange) return;
            AnimationController.PlayAnimation("move");
            if (_isMovingLeft)
            {
                Velocity = new Vector2(-_moveVelocity, Velocity.Y);
                MoveAndSlide();
                AnimationController.MoveDirection(Vector2.Left);
                AnimationController.PlayAnimation("idle");
                await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
            }
            else if (_isMovingRight)
            {
                Velocity = new Vector2(_moveVelocity, Velocity.Y);
                MoveAndSlide();
                AnimationController.MoveDirection(Vector2.Right);
                AnimationController.PlayAnimation("idle");
                await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
            }
        }
        else
        {
            Velocity = new Vector2(0, Velocity.Y);
            MoveAndSlide();
            AnimationController.PlayAnimation("idle");
            await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
            IsMoving = true;
        }
    }

    #endregion

    #region Attack

    public override void Attack()
    {
        if (!IsInAttackRange) return;
        if (!IsAttacking) return;
        IsMoving = false;
        IsAttacking = false;
        var lookAtPlayer = new Vector2(_playerControllerVariables.Position.X - Position.X, 0).Normalized();
        AnimationController.MoveDirection(lookAtPlayer);
        AnimationController.PlayAnimation("attack");
        MineActions.OnTakeDamageStarted?.Invoke(5);
        IsMoving = true;
    }

    #endregion

    #region Take Damage

    public override void TakeDamage(int damageValue)
    {
        if (IsDead) return;
        if (IsTakingDamage) return;
        IsMoving = false;
        IsTakingDamage = true;
        _knockBack = true;

        HealthSystem.ReduceEnemyHealth(damageValue, 25, this);
    }

    private async void EnemyDamageAnimation()
    {
        AnimationController.PlayAnimation("damage");
        await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
        IsTakingDamage = false;
        IsMoving = false;
        IsMoving = true;
    }

    private async Task PlayIdleAnimation()
    {
        IsMoving = false;
        AnimationController.PlayAnimation("idle");
        await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength) * 1000);
        IsMoving = true;
    }

    #endregion

    #region Death

    public override void Death()
    {
        IsDead = true;
        IsMoving = false;
        SetPhysicsProcess(false);
        AnimationController.PlayAnimation("death");
        // await Task.Delay(Mathf.CeilToInt(AnimationController.CurrentAnimationLength * 1000));
        GD.Print("ENEMY DYING");
    }

    private void OnDeathAnimationComplete(string animName)
    {
        if (animName != "death") return;
        QueueFree();
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

    #region Aggro

    private void PlayAggroAnimation()
    {
        GD.Print($"IsAggro changed {IsAggro}");
        IsMoving = false;
        AnimationController.PlayAnimation("aggro");
    }

    private void OnAggroAnimationFinished(string animName)
    {
        if (animName != "aggro") return;
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
        if (hasCollidedWithMine)
        {
            _isGrounded = true;
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
    [Export] private float _finalGravity;

    private void ApplyGravity(bool collisionBool, float delta)

    {
        if (!collisionBool)

        {
            var velocity = Velocity;
            velocity.Y += _gravity * delta;
            velocity.Y = Mathf.Clamp(velocity.Y, 0, _finalGravity);
            Velocity = velocity;
        }
    }

    #endregion

    #region Knock Back

    [Export] private bool _knockBack;

    private async void KnockBack()
    {
        if (!_knockBack) return;
        _knockBack = false;
        var playerDirection = _playerControllerVariables.PlayerDirection;
        var knockBackDirection = (playerDirection - Velocity).Normalized() * KnockBackPower;
        await ApplyKnockBack(knockBackDirection);
    }

    private async Task ApplyKnockBack(Vector2 knockBackDirection)
    {
        for (var i = 0; i < 5; i++)
        {
            Velocity = Velocity.Lerp(new Vector2(knockBackDirection.X, Velocity.Y), 0.8f);
            MoveAndSlide();
            await Task.Delay(1);
        }
    }

    #endregion

    #endregion

    private void TurnOnEnemyVisibility()
    {
        Visible = true;
    }

    private void TurnOffEnemyVisibility()
    {
        Visible = false;
    }

    private void OnLeftWallAreaCollisionEnter(Node2D body)
    {
        var hasCollidedWithMine = body == _mineGenerationVariables.MineGenView;
        var stalagmite = body as Stalagmite;
        hasCollidedWithMine |= stalagmite != null;
        if (!hasCollidedWithMine) return;
        // GD.Print("has wall on the left enter, move direction = right");
        _hasWallOnLeft = true;
    }

    private void OnLeftWallAreaCollisionExit(Node2D body)
    {
        var hasCollidedWithMine = body == _mineGenerationVariables.MineGenView;
        var stalagmite = body as Stalagmite;
        hasCollidedWithMine |= stalagmite != null;
        if (!hasCollidedWithMine) return;
        // GD.Print("has wall on the left exit, move direction = right");
        _hasWallOnLeft = false;
    }

    private void OnRightWallAreaCollisionEnter(Node2D body)
    {
        var hasCollidedWithMine = body == _mineGenerationVariables.MineGenView;
        if (!hasCollidedWithMine) return;
        // GD.Print("has wall on the right enter, move direction = left");
        _hasWallOnRight = true;
    }

    private void OnRightWallAreaCollisionExit(Node2D body)
    {
        var hasCollidedWithMine = body == _mineGenerationVariables.MineGenView;
        if (!hasCollidedWithMine) return;
        // GD.Print("has wall on the right exit, move direction = left");
        _hasWallOnRight = false;
    }

    private void UnsubscribeToActions()
    {
        OnSpawn -= SetValuesOnSpawn;
        OnAggroChanged -= PlayAggroAnimation;
        OnTakeDamage -= EnemyDamageAnimation;
    }

    public override void _ExitTree()
    {
        SetPhysicsProcess(false);
        UnsubscribeToActions();
    }
}