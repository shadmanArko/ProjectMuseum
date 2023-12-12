using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;
using Godot4CS.ProjectMuseum.Scripts.Mine.UI;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class ReferenceStorage : Node2D
{
    public static ReferenceStorage Instance;

    public override void _EnterTree()
    {
        Instance ??= this;
    }

    [Export] public SceneTransition sceneTransition;
    [Export] public AutoAnimationController autoAnimationController;
    [Export] public EnemySpawner enemySpawner;
    
    //UI//
    [Export] public MineExitPromptUi mineExitPromptUi;
    [Export] public CampExitPromptUi campExitPromptUi;
}