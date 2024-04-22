using System;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Player.Systems;

public class EnergySystem
{
	public static void RestoreFullEnergy(PlayerControllerVariables playerControllerVariables)
	{
		playerControllerVariables.PlayerEnergy = 200;
	}

	public static void RestoreEnergy(int partialValue, PlayerControllerVariables playerControllerVariables)
	{
		var energy = playerControllerVariables.PlayerEnergy;
		energy += partialValue;
		energy = Math.Clamp(energy, 0, 200);
		playerControllerVariables.PlayerEnergy = energy;
	}

	public static void ReduceEnergy(int reduceValue,int maxValue, PlayerControllerVariables playerControllerVariables)
	{
		var energy = playerControllerVariables.PlayerEnergy;
		energy -= reduceValue;
		energy = Math.Clamp(energy, 0, maxValue);
		playerControllerVariables.PlayerEnergy = energy;
	}
	
	public static async void EffectPlayerEnergy(ConsumableStatEffect statEffect, PlayerControllerVariables playerControllerVariables)
	{
		var effectDuration = statEffect.EffectDuration;
		var effectRate = Mathf.CeilToInt(statEffect.EffectAmount / statEffect.EffectDuration);
		var intervalInSeconds = statEffect.AdditiveMod switch
		{
			"FlatAdditive" => 1,
			"StaggeredAdditive" => 1000,
			_=> 0
		};
		
		for (var i = 0; i < effectDuration; i++)
		{
			await Task.Delay(intervalInSeconds);
			RestoreEnergy(effectRate, playerControllerVariables);
		}
	}

}