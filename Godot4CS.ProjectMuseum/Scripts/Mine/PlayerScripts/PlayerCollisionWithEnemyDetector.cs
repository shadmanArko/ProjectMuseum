using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerCollisionWithEnemyDetector : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    
    [Export] private Area2D _attackDetectorArea2D;
    [Export] private CollisionShape2D _collisionShape2D;
    [Export] private AnimationPlayer _animationPlayer;
    
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
        MineActions.OnPlayerMeleeAttackActionStarted += TurnOnAttackCollider;
        MineActions.OnPlayerMeleeAttackActionEnded += TurnOffAttackCollider;
    }

    private void TurnOffAttackCollider()
    {
        _collisionShape2D.Disabled = true;
        _attackDetectorArea2D.Monitoring = false;
        GD.Print("Stopped detecting collision");
    }

    private void TurnOnAttackCollider()
    {
        _collisionShape2D.Disabled = false;
        _attackDetectorArea2D.Monitoring = true;
        GD.Print("Started detecting collision");
    }

    private void OnBodyEnter(Node2D body)
    {
        var enemy = body as IDamagable;
        GD.Print($"body entered is null {enemy == null}");
        if(enemy is null) return;
        if(!_playerControllerVariables.IsAttacking) return;
        if(_playerControllerVariables.CurrentEquippedItem != Equipables.Sword) return;
        enemy.TakeDamage();
    }

    private void OnBodyExit(Node2D body)
    {
        var enemy = body as IUnit;
        GD.Print($"body exited is null {enemy == null}");
    }
}