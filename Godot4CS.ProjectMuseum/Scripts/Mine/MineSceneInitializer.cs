using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineSceneInitializer : Node
{
	[Export] private Node _rootNode;
	[Export] private string _playerScenePrefabPath;
	[Export] private string _mineSceneViewPrefabPath;
    
	public override void _Ready()
	{
		GenerateMine();
		InstantiatePlayer();
		
	}

	private void GenerateMine()
	{
		var scene = ResourceLoader.Load<PackedScene>(_mineSceneViewPrefabPath).Instantiate() as MineGenerationController;
		if (scene is null)
		{
			GD.PrintErr("COULD NOT GENERATE MINE. FATAL ERROR");
			return;
		}
		AddChild(scene);
	}
	
	private void InstantiatePlayer()
	{
		var scene = ResourceLoader.Load<PackedScene>(_playerScenePrefabPath).Instantiate();
		GD.Print($"Player Scene is null {scene is null}");
		AddChild(scene);
		MineActions.OnPlayerSpawned?.Invoke();
		GD.Print("ON PLAYER SPAWNED CALLED");
	}
            
}