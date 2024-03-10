using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Camera;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;
using Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Mine.UI;
using Godot4CS.ProjectMuseum.Scripts.Mine.UI.DamageSystem;
using Godot4CS.ProjectMuseum.Scripts.Museum.Tutorial_System;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class ReferenceStorage : Node
{
    public static ReferenceStorage Instance;

    public PlayerControllerVariables PlayerControllerVariables;
    public MineGenerationVariables MineGenerationVariables;
    
    [Export] public SceneTransition SceneTransition;
    [Export] public AutoAnimationController AutoAnimationController;
    [Export] public EnemySpawner EnemySpawner;
    [Export] public MiniGameController MiniGameController;
    [Export] public MineTutorial MineTutorial;
    [Export] public CameraController CameraController;
    [Export] public Plugins.Time_System.MineTimeSystem.Scripts.MineTimeSystem MineTimeSystem;
    [Export] public SceneLoader SceneLoader;
    [Export] public DamageSystem DamageSystem;
    
    //UI//
    [Export] public MineExitPromptUi MineExitPromptUi;
    [Export] public CampExitPromptUi CampExitPromptUi;
    [Export] public DiscoveredArtifactVisualizer DiscoveredArtifactVisualizer;
    [Export] public MinePopUp MinePopUp;
    [Export] public MineUiController MineUiController;
    
    //Particle Effects//
    [Export] public string DepletedParticleExplosion;

    [Export] public MineSceneTooltip Tooltip;
    
    public override void _EnterTree()
    {
        Instance ??= this;
        PlayerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        MineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }
}