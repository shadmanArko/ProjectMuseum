using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class EnemyCollisionDetector : Area2D
{
    [Export] private CharacterBody2D _characterBody2D;
    
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    public override void _Ready()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    public void AttackPlayer(Node2D body)
    {
        GD.Print("Player entered in the area");
        var slime = _characterBody2D as IUnit;
        
        if(slime!.State != EnemyState.DigIn)
            slime!.State = EnemyState.Attack;
    }
}