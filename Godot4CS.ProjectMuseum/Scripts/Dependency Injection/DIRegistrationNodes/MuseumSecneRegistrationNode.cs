using System.Collections.Generic;
using Godot;
using Godot.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection.DIRegistrationNodes;

public partial class MuseumSecneRegistrationNode : Node, IServicesConfigurator
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGodotServices();
        services.AddSingleton(typeof(List<ExhibitPlacementConditionData>));
    }
}