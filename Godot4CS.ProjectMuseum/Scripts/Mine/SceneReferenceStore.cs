using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class SceneReferenceStore : Node
{
	private const string MineDiInstallerScenePath = "res://Scenes/Mine/Main Scene/MineSceneDiInstaller.tscn";
	private const string AlternateTapMiniGameScenePath = "res://Scenes/Mine/Sub Scenes/MiniGames/AlternateTapMiniGame.tscn";

	public override void _Ready()
	{
		GD.Print("Scene Reference store initialized");
		SubscribeToActions();
	}

	private void SubscribeToActions()
	{
		//MineActions.OnMiniGameLoad += LoadMiniGameScene;
	}

	private void LoadMiniGameScene(Node2D parentNode)
	{
		var scene = ResourceLoader.Load<PackedScene>(AlternateTapMiniGameScenePath).Instantiate() as AlternateTapMiniGame;
		if (scene is null)
		{                                                                                                            
			GD.PrintErr("COULD NOT instantiate Alternate tap mini game scene. FATAL ERROR");
			return;
		}

		parentNode.AddChild(scene);
		
	}
}