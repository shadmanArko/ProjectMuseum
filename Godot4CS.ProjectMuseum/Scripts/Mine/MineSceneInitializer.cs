using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineSceneInitializer : Node
{
	[Export] private Node _rootNode;
	[Export] private string _playerScenePrefabPath;
	[Export] private string _mineScenePrefabPath;
	[Export] private string _mineBackgroundScenePath;
	[Export] private string _mineUiScenePath;
    
	public override void _Ready()
	{
		//InstantiateMineUi();
		GenerateMine();
		InstantiatePlayer();
	}

	private void InstantiateMineUi()
	{
		var scene = ResourceLoader.Load<PackedScene>(_mineUiScenePath).Instantiate() as MineUiController;
		if (scene is null)
		{
			GD.PrintErr("COULD NOT INSTANTIATE MINE UI. FATAL ERROR");
			return;
		}
		AddChild(scene);
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