using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public partial class DiTest4 : Node
{
	private readonly RandomGuidGenerator _randomGuidGeneratorFirst;
	private readonly RandomGuidGenerator _randomGuidGeneratorSecond;
	private readonly RandomGuidGenerator _randomGuidGeneratorThird;

	public DiTest4()
	{
		_randomGuidGeneratorFirst = ServiceRegistry.Resolve<RandomGuidGenerator>();
		_randomGuidGeneratorSecond = ServiceRegistry.Resolve<RandomGuidGenerator>();
		_randomGuidGeneratorThird = ServiceRegistry.Resolve<RandomGuidGenerator>();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// await Task.Delay(5000);
		// var randomGuidGenerator1 = ServiceRegistry.Resolve<RandomGuidGenerator>();
		// //GD.Print(ServiceRegistry.Services.Count);
		// GD.Print(randomGuidGenerator1.RandomGuid);
		// await Task.Delay(1000);
		// var randomGuidGenerator2 = ServiceRegistry.Resolve<RandomGuidGenerator>();
		// //GD.Print(ServiceRegistry.Services.Count);
		// GD.Print(randomGuidGenerator2.RandomGuid);
		
		GD.Print(_randomGuidGeneratorFirst.RandomGuid);
		GD.Print(_randomGuidGeneratorSecond.RandomGuid);
		GD.Print(_randomGuidGeneratorThird.RandomGuid);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
