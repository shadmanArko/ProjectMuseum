using Godot;
using System;
using Godot.DependencyInjection;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Microsoft.Extensions.DependencyInjection;

public partial class DependencyRegistrationNode : Node, IServicesConfigurator
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton<RandomGuidGenerator>();
		// GD.Print("entered into di");
		// services.AddGodotServices();
		// services.AddSingleton<>();
		// services.AddTransient<>();
		//services.AddTransient<IService, Service>();
	}
}
