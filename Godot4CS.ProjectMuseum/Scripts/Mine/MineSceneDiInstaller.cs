using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineSceneDiInstaller : Node
{
	public MineSceneDiInstaller()
	{
		ServiceRegistry.RegisterSingleton<PlayerControllerVariables>();
		ServiceRegistry.RegisterSingleton<MineGenerationVariables>();
		ServiceRegistry.RegisterSingleton<global::ProjectMuseum.Models.Mine>();
		ServiceRegistry.RegisterSingleton<MineCellCrackMaterial>();
		ServiceRegistry.RegisterSingleton<RawArtifactDTO>();
		
		ServiceRegistry.RegisterSingleton<InventoryDTO>();
		ServiceRegistry.RegisterSingleton<ConsumableDTO>();
		ServiceRegistry.RegisterSingleton<WallPlaceableDTO>();
		ServiceRegistry.RegisterSingleton<CellPlaceableDTO>();
		ServiceRegistry.RegisterSingleton<EquipableDTO>();
		
		ServiceRegistry.Initialize();
	}
}