using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public partial class DiTest03 : Node3D
{
	private readonly RandomGuidGenerator _randomGuidGenerator;

	public DiTest03()
	{
		GD.Print("_randomGuidGenerator.RandomGuid");
		//_randomGuidGenerator = _randomGuidGenerator;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("_randomGuidGenerator.RandomGuid");
		//var	_randomGuidGenerator = ServiceRegistry.Resolve<RandomGuidGenerator>();
		GD.Print(_randomGuidGenerator.RandomGuid);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
