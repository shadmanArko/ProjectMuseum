using Godot;
using Godot.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public class DependencyRegistrationNode : Node, IServicesConfigurator
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGodotServices();
        //services.AddTransient<IService, Service>();
    }
}