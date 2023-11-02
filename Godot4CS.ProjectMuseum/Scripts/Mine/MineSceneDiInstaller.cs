using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineSceneDiInstaller : Node
{
	public MineSceneDiInstaller()
	{
		GD.Print("Museum DI Installer initialized");
		ServiceRegistry.RegisterSingleton<PlayerControllerVariables>();
		ServiceRegistry.RegisterSingleton<MineGenerationVariables>();
		ServiceRegistry.RegisterSingleton<global::ProjectMuseum.Models.Mine>();
		ServiceRegistry.Initialize();
		GD.Print("Service Registry initialized");
	}
}