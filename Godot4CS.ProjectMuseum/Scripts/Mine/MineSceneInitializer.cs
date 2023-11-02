using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts;

public partial class MineSceneInitializer : Node
{
	[Export] private Node _rootNode;
	[Export] private string _playerScenePrefabPath;
	[Export] private string _mineScenePrefabPath;
    
	public override void _Ready()
	{
		GenerateMine();
		InstantiatePlayer();
	}

	private void GenerateMine()
	{
		var scene = ResourceLoader.Load<PackedScene>(_mineScenePrefabPath).Instantiate() as MineGenerationController;
		if (scene is null)
		{
			GD.PrintErr("COULD NOT GENERATE MINE. FATAL ERROR");
			return;
		}
		AddChild(scene);
		scene.GenerateMine();
	}
	
	private void InstantiatePlayer()
	{
		var scene = ResourceLoader.Load<PackedScene>(_playerScenePrefabPath).Instantiate();
		GD.Print($"Player Scene is null {scene is null}");
		AddChild(scene);
	}
}