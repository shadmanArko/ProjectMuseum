using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts;

public partial class MineSceneDiInstaller : Node
{
	public MineSceneDiInstaller()
	{
		GD.Print("Museum DI Installer initialized");
		ServiceRegistry.RegisterSingleton<MineGenerationController>();
		ServiceRegistry.RegisterSingleton<PlayerControllerVariables>();
		ServiceRegistry.Initialize();
		GD.Print("Service Registry initialized");
	}
}