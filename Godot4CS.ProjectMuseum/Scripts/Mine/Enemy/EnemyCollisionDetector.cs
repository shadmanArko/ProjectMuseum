using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class EnemyCollisionDetector : Node2D
{
    [Export] private Enemy _enemy;
    
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    
    [Export] private Timer _attackCooldownTimer;
    [Export] private bool _enemyCooldown;

    public override void _Ready()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }
    
    private void OnEnemyAttackCooldownTimeOut()
    {
        _enemyCooldown = false;
        if (_enemy.IsInAttackRange && _enemy.Phase == EnemyPhase.Combat)
            _enemy.IsAttacking = true;
    }
    
    #region Attack Range

    private void OnPlayerEnterAttackRange(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        if(_enemyCooldown) return;
        _enemy.IsInAttackRange = true;
        _enemy.IsMoving = false;
        _enemy.Phase = EnemyPhase.Combat;
        // _enemy.Attack();
        _enemy.IsAttacking = true;
        _enemyCooldown = true;
    }

    private void OnPlayerExitAttackRange(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        _enemy.IsInAttackRange = false;
        _enemy.IsMoving = true;
        _enemy.IsAttacking = false;
        _enemy.Phase = EnemyPhase.Chase;
        GD.Print($"Player exited ATTACK region, isAttacking:{_enemy.IsAttacking}");
    }

    #endregion
    
    #region Chase Range

    private void OnPlayerEnterChaseRange(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        _enemy.IsAggro = true;
        GD.Print($"Player entered chase region, enemy.IsAggro:{_enemy.IsAggro}");
    }

    private void OnPlayerExitChaseRange(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        _enemy.IsAggro = false;
        GD.Print($"Player exited chase region, enemy.IsAggro:{_enemy.IsAggro}");
    }

    #endregion

}