using Godot;
using Godot.DependencyInjection;
using Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;
using Microsoft.Extensions.DependencyInjection;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection.DIRegistrationNodes;

public partial class MineSceneRegistrationNode : Node, IServicesConfigurator
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGodotServices();
        // services.AddSingleton<>();
        // services.AddTransient<>();
        //services.AddTransient<IService, Service>();

        services.AddSingleton<PlayerController>();
    }
}