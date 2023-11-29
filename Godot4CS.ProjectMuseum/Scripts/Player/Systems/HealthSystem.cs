using System;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Player.Systems;

public class HealthSystem
{
	public static void RestoreFullHealth(int maxValue, PlayerControllerVariables playerControllerVariables)
	{
		playerControllerVariables.PlayerHealth = maxValue;
	}

	public static void RestoreHealth(int partialValue, int maxValue, PlayerControllerVariables playerControllerVariables)
	{
		var health = playerControllerVariables.PlayerHealth;
		health += partialValue;
		health = Math.Clamp(health, 0, maxValue);
		playerControllerVariables.PlayerHealth = health;
	}

	public static void ReduceHealth(int reduceValue, int maxValue, PlayerControllerVariables playerControllerVariables)
	{
		var health = playerControllerVariables.PlayerHealth;
		health -= reduceValue;
		health = Math.Clamp(health, 0, maxValue);
		playerControllerVariables.PlayerHealth = health;
	}
	
}