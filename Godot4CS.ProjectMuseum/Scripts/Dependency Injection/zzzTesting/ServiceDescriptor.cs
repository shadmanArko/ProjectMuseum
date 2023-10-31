using System;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public class ServiceDescriptor
{
    public Type ServiceType { get; }
    public object Implementation { get; internal set; }
    public ServiceLifetime ServiceLifetime { get; }

    public ServiceDescriptor(object implementation, ServiceLifetime serviceLifetime)
    {
        ServiceType = implementation.GetType();
        Implementation = implementation;
        ServiceLifetime = serviceLifetime;
    }

    public ServiceDescriptor(Type serviceType, ServiceLifetime serviceLifetime)
    {
        ServiceType = serviceType;
        ServiceLifetime = serviceLifetime;
    }
}