using Godot;
using System;
using System.Collections.Generic;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.GameData;
using ProjectMuseum.Models;

public partial class MuseumSceneDiInstaller : Node
{
	

	public override void _Ready()
	{
		GD.Print($"museum scene di installer ready");
		ServiceRegistry.RegisterSingleton<List<ExhibitPlacementConditionData>>();
		ServiceRegistry.RegisterSingleton<MuseumRunningDataContainer>();
		ServiceRegistry.RegisterSingleton<MuseumGameData>();
		
		ServiceRegistry.Initialize();
	}
}
