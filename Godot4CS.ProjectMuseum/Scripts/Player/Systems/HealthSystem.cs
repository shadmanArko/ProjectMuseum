using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Player.Systems;

public class HealthSystem
{
	public static void RestorePlayerFullHealth(int maxValue, PlayerControllerVariables playerControllerVariables)
	{
		playerControllerVariables.PlayerHealth = maxValue;
	}

	public static void RestorePlayerHealth(int partialValue, int maxValue, PlayerControllerVariables playerControllerVariables)
	{
		var health = playerControllerVariables.PlayerHealth;
		health += partialValue;
		health = Math.Clamp(health, 0, maxValue);
		playerControllerVariables.PlayerHealth = health;
	}

	public static void ReducePlayerHealth(int reduceValue, int maxValue, PlayerControllerVariables playerControllerVariables)
	{
		var health = playerControllerVariables.PlayerHealth;
		health -= reduceValue;
		health = Math.Clamp(health, 0, maxValue);
		playerControllerVariables.PlayerHealth = health;
	}

	public static void ReduceEnemyHealth(int reduceValue, int maxValue, Slime slime)
	{
		slime.Health -= reduceValue;
		slime.Health = Math.Clamp(slime.Health, slime.Health, maxValue);
	}
	
}