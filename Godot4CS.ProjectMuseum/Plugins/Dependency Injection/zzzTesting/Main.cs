using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Godot;
using Microsoft.Extensions.DependencyInjection;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public partial class Main : Node
{
    // private readonly IServiceCollection _serviceCollection;
    //
    // private Container _container;

    public Main()
    {
        GD.Print("gg");
        // _serviceCollection = new ServiceCollection();
        var services = new DiServiceCollection();
    
        services.RegisterSingleton<RandomGuidGenerator>();
        // services.RegisterSingleton(new RandomGuidGenerator());
        var container = services.GenerateContainer();

        var serviceFirst = container.GetService<RandomGuidGenerator>();
        var serviceSecond = container.GetService<RandomGuidGenerator>();
        
        GD.Print(serviceFirst.RandomGuid);
        GD.Print(serviceSecond.RandomGuid);
        // Console.WriteLine(serviceFirst.RandomGuid);
        // Console.WriteLine(serviceSecond.RandomGuid);
    }
    private static void Main1()
    {
        
        
        
        
    }

    // public void ConfigServices()
    // {
    //     _serviceCollection.AddSingleton<RandomGuidGenerator>();
    //
    // }
}