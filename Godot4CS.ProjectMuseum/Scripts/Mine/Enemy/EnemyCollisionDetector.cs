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
        GD.Print("Enemy cooldown is false");
        _enemyCooldown = false;
    }

    public void AttackPlayer(Node2D body)
    {
        var player = body as PlayerController;
        if(player == null) return;
        if(_playerControllerVariables.IsDead) return;
        

        if (_enemy!.State != EnemyState.DigIn && !_enemyCooldown)
        {
            _enemy!.State = EnemyState.Attack;
            _enemy.Attack();
            _enemyCooldown = true;
            GD.Print("Enemy cooldown is true");
            GD.Print("ENEMY ATTACKING PLAYER");
        }
    }

    #region Wall Collision

    private void OnCollideWithWallEntered(Node body)
    {
        var tilemap = body as TileMap;
        if (tilemap == null)
        {
            GD. Print("tilemap is null");
            return;
        }

        // if (tilemap == _mineGenerationVariables.MineGenView.TileMap)
        // {
        //     GD.Print("TileMAPS ARE MATCH");
        //     var currentTilePos = _mineGenerationVariables.MineGenView.LocalToMap(_character.Position);
        //     
        //     var cell = _mineGenerationVariables.GetCell(currentTilePos);
        //     var leftCell = _mineGenerationVariables.GetCell(new Vector2I(cell.PositionX - 1, cell.PositionY));
        //     var rightCell = _mineGenerationVariables.GetCell(new Vector2I(cell.PositionX + 1, cell.PositionY));
        //
        //     if (leftCell != null)
        //     {
        //         if (!leftCell.IsBreakable && !leftCell.IsBroken)
        //         {
        //             var cellSize = _mineGenerationVariables.Mine.CellSize;
        //             _character.NavAgent.TargetPosition =
        //                 new Vector2(leftCell.PositionX * cellSize, leftCell.PositionY * cellSize);
        //             GD.Print("Setting left cell as target");
        //             return;
        //         }
        //     }
        //     
        //     if(rightCell != null)
        //     {
        //         if (!rightCell.IsBreakable && !rightCell.IsBroken)
        //         {
        //             var cellSize = _mineGenerationVariables.Mine.CellSize;
        //             _character.NavAgent.TargetPosition =
        //                 new Vector2(rightCell.PositionX * cellSize, rightCell.PositionY * cellSize);
        //             GD.Print("Setting right cell as target");
        //             return;
        //         }
        //     }
        //
        //
        // }
        GD.Print("Colliding with wall");
    }
    
    private void OnCollideWithWallExited(Node body)
    {
        GD.Print("Colliding with wall");
    }

    #endregion
}