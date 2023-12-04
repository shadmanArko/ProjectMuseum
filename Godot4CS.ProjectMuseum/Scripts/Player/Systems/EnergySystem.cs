using System;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Player.Systems;

public class EnergySystem
{
	public static void RestoreFullEnergy(int maxValue, PlayerControllerVariables playerControllerVariables)
	{
		playerControllerVariables.PlayerEnergy = maxValue;
	}

	public static void RestoreEnergy(int partialValue,int maxValue, PlayerControllerVariables playerControllerVariables)
	{
		var energy = playerControllerVariables.PlayerEnergy;
		energy += partialValue;
		energy = Math.Clamp(energy, 0, maxValue);
		playerControllerVariables.PlayerEnergy = energy;
	}

	public static void ReduceEnergy(int reduceValue,int maxValue, PlayerControllerVariables playerControllerVariables)
	{
		var energy = playerControllerVariables.PlayerEnergy;
		energy -= reduceValue;
		energy = Math.Clamp(energy, 0, maxValue);
		playerControllerVariables.PlayerEnergy = energy;
	}
}