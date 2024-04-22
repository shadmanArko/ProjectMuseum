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
        MineActions.OnMeleeAttackActionStarted += TurnOnAttackCollider;
        MineActions.OnMeleeAttackActionEnded += TurnOffAttackCollider;
    }

    #endregion

    private void TurnOffAttackCollider()
    {
        _collisionShape2D.Disabled = true;
        _attackDetectorArea2D.Monitoring = false;
        // GD.Print("Stopped detecting collision");
    }

    private void TurnOnAttackCollider()
    {
        _collisionShape2D.Disabled = false;
        _attackDetectorArea2D.Monitoring = true;
        // GD.Print("Started detecting collision");
    }

    private void OnBodyEnter(Node2D body)
    {
        var enemy = body as IDamagable;
        if(enemy is null) return;
        
        if(!_playerControllerVariables.IsAttacking) return;
        if(_playerControllerVariables.CurrentEquippedItemSlot != 0) return;
        enemy.TakeDamage();
        GD.Print($"PLAYER ATTACKING ENEMY {enemy}");
    }

    private void OnBodyExit(Node2D body)
    {
        var enemy = body as IUnit;
        GD.Print($"body exited is null {enemy == null}");
    }
}