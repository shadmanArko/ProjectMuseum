using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineSceneDiInstaller : Node
{
	public MineSceneDiInstaller()
	{
		GD.Print("Mine DI Installer initialized");
		ServiceRegistry.RegisterSingleton<PlayerControllerVariables>();
		ServiceRegistry.RegisterSingleton<MineGenerationVariables>();
		ServiceRegistry.RegisterSingleton<global::ProjectMuseum.Models.Mine>();
		ServiceRegistry.RegisterSingleton<MineCellCrackMaterial>();
		ServiceRegistry.RegisterSingleton<List<RawArtifactDescriptive>>();
		ServiceRegistry.RegisterSingleton<List<RawArtifactFunctional>>();
		ServiceRegistry.Initialize();
		GD.Print("Service Registry initialized");
	}
}