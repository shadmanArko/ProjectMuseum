using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;
using Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;
using Godot4CS.ProjectMuseum.Scripts.Mine.UI;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class ReferenceStorage : Node
{
    public static ReferenceStorage Instance;

    public override void _EnterTree()
    {
        Instance ??= this;
    }

    [Export] public SceneTransition sceneTransition;
    [Export] public AutoAnimationController autoAnimationController;
    [Export] public EnemySpawner enemySpawner;
    [Export] public MiniGameController miniGameController;
    
    //UI//
    [Export] public MineExitPromptUi mineExitPromptUi;
    [Export] public CampExitPromptUi campExitPromptUi;
    [Export] public DiscoveredArtifactVisualizer DiscoveredArtifactVisualizer;
    
    //Particle Effects//
    [Export] public string DepletedParticleExplosion;

    [Export] public MineSceneTooltip Tooltip;
}