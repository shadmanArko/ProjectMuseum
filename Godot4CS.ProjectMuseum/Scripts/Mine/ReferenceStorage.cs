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

    [Export] public SceneTransition SceneTransition;
    [Export] public AutoAnimationController AutoAnimationController;
    [Export] public EnemySpawner EnemySpawner;
    [Export] public MiniGameController MiniGameController;
    
    //UI//
    [Export] public MineExitPromptUi MineExitPromptUi;
    [Export] public CampExitPromptUi CampExitPromptUi;
    [Export] public DiscoveredArtifactVisualizer DiscoveredArtifactVisualizer;
    [Export] public MinePopUp MinePopUp;
    
    //Particle Effects//
    [Export] public string DepletedParticleExplosion;

    [Export] public MineSceneTooltip Tooltip;
}