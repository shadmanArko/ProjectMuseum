using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerCollisionWithEnemyDetector : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    
    [Export] private Area2D _attackDetectorArea2D;
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

    private void TurnOffAttackCollider()
    {
        _attackDetectorArea2D.Monitoring = false;
        _playerControllerVariables.IsAttacking = false;
        GD.Print("Stopped detecting enemy collision");
    }

    private async void TurnOnAttackCollider()
    {
        _attackDetectorArea2D.Monitoring = true;
        _playerControllerVariables.IsAttacking = true;
        GD.Print("Started detecting enemy collision");
        await Task.Delay(Mathf.CeilToInt(_playerControllerVariables.Player.animationController.CurrentAnimationLength) * 1000);
        _playerControllerVariables.IsAttacking = false;
        _attackDetectorArea2D.Monitoring = false;
    }

    private void OnBodyEnter(Node2D body)
    {
        var enemy = body as IDamagable;
        if(enemy is null) return;
        GD.Print($"is attacking is {_playerControllerVariables.IsAttacking}");
        if (!_playerControllerVariables.IsAttacking)
        {
            GD.Print("is attacking not true. returning");
            return;
        }
        // if(_playerControllerVariables.CurrentEquippedItemSlot != 0) return;
        enemy.TakeDamage();
        GD.Print($"PLAYER ATTACKING ENEMY {enemy}");
    }

    private void OnBodyExit(Node2D body)
    {
        var enemy = body as IUnit;
        GD.Print($"body exited is null {enemy == null}");
    }
}