using System.Collections.Generic;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public class DiServiceCollection
{
    private List<ServiceDescriptor> _serviceDescriptors = new List<ServiceDescriptor>();
    
    public void RegisterSingleton<TService>()
    {
        _serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), ServiceLifetime.Singleton));
    }
    
    public void RegisterSingleton<TService>(TService implementation)
    { 
        _serviceDescriptors.Add(new ServiceDescriptor(implementation, ServiceLifetime.Singleton));
    }

    public DiContainer GenerateContainer()
    {
        return new DiContainer(_serviceDescriptors);
    }
}