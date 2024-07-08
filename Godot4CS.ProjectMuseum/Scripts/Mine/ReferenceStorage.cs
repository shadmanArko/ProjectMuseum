using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Loading_Bar;
using Godot4CS.ProjectMuseum.Scripts.Mine.Camera;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;
using Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;
using Godot4CS.ProjectMuseum.Scripts.Mine.ItemCollectionLogSystem;
using Godot4CS.ProjectMuseum.Scripts.Mine.MineSettings;
using Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;
using Godot4CS.ProjectMuseum.Scripts.Mine.Operations;
using Godot4CS.ProjectMuseum.Scripts.Mine.ParticleEffects;
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
    [Export] public LoadingBarManager LoadingBarManager;
    [Export] public ParticleEffectSystem ParticleEffectSystem;
    [Export] public InventoryManager InventoryManager;
    [Export] public MinePauseManager MinePauseManager;
    [Export] public LogMessageController LogMessageController;
    [Export] public ScreenShakeController ScreenShakeController;
    [Export] public InventoryItemBuilder InventoryItemBuilder;
    
    //UI//
    [Export] public MineExitPromptUi MineExitPromptUi;
    [Export] public CampExitPromptUi CampExitPromptUi;
    [Export] public DiscoveredArtifactVisualizer DiscoveredArtifactVisualizer;
    [Export] public MinePopUp MinePopUp;
    [Export] public MineUiController MineUiController;
    [Export] public InventoryViewController InventoryViewController;
    
    //Scene Paths//
    [Export] public string DepletedParticleExplosion = "res://Scenes/Mine/Sub Scenes/Particle Effects/DepletedParticleExplosion.tscn";
    public string DynamiteExplosionScenePath = "res://Scenes/Mine/Sub Scenes/Particle Effects/DynamiteExplosion.tscn";
    [Export] public string ToolbarSlotScenePath = "res://Scenes/Mine/Sub Scenes/UI/ToolbarSlot.tscn";
    [Export] public string ItemDropScenePath = "res://Scenes/Mine/Sub Scenes/Items/ItemDrop.tscn";
    public string ItemCardScenePath = "res://Scenes/Mine/Sub Scenes/Log Message System/LogMessageCard.tscn";
    public string BoulderScenePath = "res://Scenes/Mine/Sub Scenes/Special Walls/Boulder.tscn";
    
    [Export] public MineSceneTooltip Tooltip;
    
    public override void _EnterTree()
    {
        Instance ??= this;
        PlayerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        MineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }
}