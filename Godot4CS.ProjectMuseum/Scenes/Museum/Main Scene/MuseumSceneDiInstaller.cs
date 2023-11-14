using Godot;
using System;
using System.Collections.Generic;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using ProjectMuseum.Models;

public partial class MuseumSceneDiInstaller : Node
{
	public MuseumSceneDiInstaller()
	{
		ServiceRegistry.RegisterSingleton<List<ExhibitPlacementConditionData>>();
		
		
		ServiceRegistry.Initialize();
	}
}
