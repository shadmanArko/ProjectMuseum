using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerCollisionWithEnemyDetector : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    
    [Export] private Area2D _leftAttackDetector;
    [Export] private Area2D _rightAttackDetector;
    [Export] private Area2D _topAttackDetector;
    [Export] private Area2D _bottomAttackDetector;
    
    [Export] private CollisionShape2D _collisionShape2D;
    [Export] private AnimationPlayer _animationPlayer;

    #region Initializers

    public override void _Ready()
    {
        InitializeDiReferences();
        SubscribeToActions();
    }
    
    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }

    private void SubscribeToActions()
    {
        // MineActions.OnMeleeAttackActionStarted += TurnOnAttackCollider;
        // MineActions.OnMeleeAttackActionEnded += TurnOffAttackCollider;
    }

    #endregion

    private async void TurnOnRequiredAttackCollider()
    {
        GD.Print("Calling required attack collider method");
        var mouseDirection = _playerControllerVariables.MouseDirection;
        if (mouseDirection == Vector2I.Left)
        {
            GD.Print("LEFT DETECTOR turned on");
            await TurnOnAttackCollider(_leftAttackDetector);
        }
        else if (mouseDirection == Vector2I.Right)
        {
            GD.Print("RIGHT DETECTOR turned on");
            await TurnOnAttackCollider(_rightAttackDetector);
        }
        else if (mouseDirection == Vector2I.Up)
        {
            GD.Print("UP DETECTOR turned on");
            await TurnOnAttackCollider(_topAttackDetector);
        }
        else if (mouseDirection == Vector2I.Down)
        {
            GD.Print("BOTTOM DETECTOR turned on");
            await TurnOnAttackCollider(_bottomAttackDetector);
        }
    }

    private async Task TurnOnAttackCollider(Area2D attackDetector)
    {
        attackDetector.Monitoring = true;
        _playerControllerVariables.IsAttacking = true;
        GD.Print("Started detecting enemy collision");
        GD.Print($"is attacking: {_playerControllerVariables.IsAttacking}");
        await Task.Delay(Mathf.CeilToInt(_playerControllerVariables.Player.AnimationController.CurrentAnimationLength) * 1000);
        _playerControllerVariables.IsAttacking = false;
        attackDetector.Monitoring = false;
        GD.Print($"is attacking: {_playerControllerVariables.IsAttacking}");
        TurnOffAttackCollider(attackDetector);
    }
    
    private void TurnOffAttackCollider(Area2D attackDetector)
    {
        attackDetector.Monitoring = false;
        _playerControllerVariables.IsAttacking = false;
        GD.Print("Stopped detecting enemy collision");
    }
    
    private void OnBodyEnter(Node2D body)
    {
        var enemy = body as IDamageable;
        if(enemy is null) return;
        GD.Print($"ON BODY ENTER isAttacking:{_playerControllerVariables.IsAttacking}");
        if (!_playerControllerVariables.IsAttacking)
        {
            GD.Print("is attacking not true. returning");
            return;
        }
        // if(_playerControllerVariables.CurrentEquippedItemSlot != 0) return;
        enemy.TakeDamage(_playerControllerVariables.EnemyDamagePoint);
        GD.Print($"PLAYER ATTACKING ENEMY {enemy}");
    }

    private void OnBodyExit(Node2D body)
    {
        var enemy = body as IUnit;
        GD.Print($"body exited is null {enemy == null}");
    }
}