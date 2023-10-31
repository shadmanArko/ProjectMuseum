using Godot;
using System;
using Godot.DependencyInjection.Attributes;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public partial class DiTest3 : Node
{
	[Inject] public RandomGuidGenerator _randomGuidGenerator;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(_randomGuidGenerator.RandomGuid);
		
	}
	[Inject]
	public void Inject(RandomGuidGenerator randomGuidGenerator)
	{
		_randomGuidGenerator = randomGuidGenerator;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
