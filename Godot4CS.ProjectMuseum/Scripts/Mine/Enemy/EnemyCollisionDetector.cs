using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
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
    }

    // private void AttackPlayer(Node2D body)
    // {
    //     var player = body as PlayerController;
    //     if(player == null) return;
    //     if(_playerControllerVariables.IsDead) return;
    //     
    //
    //     // if (_enemy!.State != EnemyState.DigIn && !_enemyCooldown)
    //     // {
    //     //     _enemy!.State = EnemyState.Attack;
    //     //     _enemy.Attack();
    //     //     _enemyCooldown = true;
    //     //     GD.Print("Enemy cooldown is true");
    //     //     GD.Print("ENEMY ATTACKING PLAYER");
    //     // }
    // }
    
    #region Attack Range

    private void OnPlayerEnterAttackRange(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        if(_enemyCooldown) return;
        _enemy.IsAttacking = true;
        var lookAtPlayer = new Vector2(_playerControllerVariables.Position.X - Position.X, 0).Normalized();
        _enemy.IsAttacking = true;
        _enemy.AnimationController.MoveDirection(lookAtPlayer);
        _enemy.AnimationController.PlayAnimation("attack");
        player.TakeDamage();
        _enemyCooldown = true;
        GD.Print($"Player entered ATTACK region, isAttacking:{_enemy.IsAttacking}");
    }

    private void OnPlayerExitAttackRange(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        _enemy.IsAttacking = false;
        GD.Print($"Player exited ATTACK region, isAttacking:{_enemy.IsAttacking}");
    }

    #endregion

    
}