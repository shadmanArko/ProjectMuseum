using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class EnemySpawner : Node2D
{
    [Export] private Node _parentNode;
    [Export] private string _slimePrefabPath;

    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    public override void _EnterTree()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    public void OnTimeEndSpawnEnemy()
    {
        GD.Print("Spawning new enemy");
        var scene = ResourceLoader.Load<PackedScene>(_slimePrefabPath).Instantiate();
        GD.Print($"Slime Scene is null {scene is null}");
        _parentNode.AddChild(scene);
        var enemy = scene as IUnit;
        if(enemy == null)
            GD.PrintErr("Enemy is null");
        enemy!.IsAffectedByGravity = true;
    }
    
    #region For Testing

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("Enemy"))
        {
            OnTimeEndSpawnEnemy();
        }
    }

    #endregion
}