using Godot;
using Godot.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection.DIRegistrationNodes;

public partial class MuseumSecneRegistrationNode : Node, IServicesConfigurator
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGodotServices();
    }
}