using Microsoft.Extensions.DependencyInjection;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public static class ServiceRegistry
{
    public static ServiceCollection Services {get;} = new ServiceCollection();

    private static ServiceProvider _provider;
    
    public static void Initialize() 
    {
        _provider = Services.BuildServiceProvider();
    }
    
    public static void RegisterSingleton<T>() where T : class
    {
        Services.AddSingleton<T>(); 
    }
    
    public static void RegisterTransient<T>() where T : class
    {
        Services.AddTransient<T>(); 
    }

    public static T Resolve<T>() 
    {
        return _provider.GetService<T>();
    }

    

}