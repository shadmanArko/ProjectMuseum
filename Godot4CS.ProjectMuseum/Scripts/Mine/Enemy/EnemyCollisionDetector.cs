using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class EnemyCollisionDetector : Area2D
{
    [Export] private Enemy _enemy;
    
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    
    [Export] private Timer _enemyCooldownTimer;
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

    public void AttackPlayer(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        if(_playerControllerVariables.IsDead) return;
        

        // if (_enemy!.State != EnemyState.DigIn && !_enemyCooldown)
        // {
        //     _enemy!.State = EnemyState.Attack;
        //     _enemy.Attack();
        //     _enemyCooldown = true;
        //     GD.Print("Enemy cooldown is true");
        //     GD.Print("ENEMY ATTACKING PLAYER");
        // }
    }

    #region Chase Area

    private void OnPlayerEnteredIntoRange()
    {
        _enemy.IsAggro = true;
    }

    private void OnPlayerExitedFromRange()
    {
        _enemy.IsAggro = false;
    }

    #endregion
    
}