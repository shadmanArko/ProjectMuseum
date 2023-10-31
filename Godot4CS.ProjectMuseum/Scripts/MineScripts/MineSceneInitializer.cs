using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.MineScripts;
using Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

public partial class MineSceneInitializer : Node
{
	[Export] private Node _rootNode;
	[Export] private string _playerScenePrefabPath;
	[Export] private string _mineScenePrefabPath;
	
	private PlayerController _playerController;
	private MineGenerationController _mineGenerationController;

	[Export] private string _playerControllerScriptPath;
	
	public override void _Ready()
	{
		InitializeDiReferences();
		//GenerateMine();
		InstantiatePlayer();
	}

	private void InitializeDiReferences()
	{
		_playerController = ServiceRegistry.Resolve<PlayerController>();
		_mineGenerationController = ServiceRegistry.Resolve<MineGenerationController>();
	}

	private void GenerateMine()
	{
		var scene = ResourceLoader.Load<PackedScene>(_mineScenePrefabPath).Instantiate();
		GD.Print($"Mine Scene is null {scene is null}");
		
		scene.SetScript(_mineGenerationController);
		_mineGenerationController.GenerateMine();
	}
	
	private void InstantiatePlayer()
	{
		var scene = ResourceLoader.Load<PackedScene>(_playerScenePrefabPath).Instantiate();
		GD.Print($"Player Scene is null {scene is null}");
		AddChild(scene);
	}
}
