using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Player.Systems;

public class HealthSystem
{
	public static void RestorePlayerFullHealth(PlayerControllerVariables playerControllerVariables)
	{
		playerControllerVariables.PlayerHealth = 200;
	}

	public static void RestorePlayerHealth(int partialValue, PlayerControllerVariables playerControllerVariables)
	{
		var health = playerControllerVariables.PlayerHealth;
		health += partialValue;
		health = Math.Clamp(health, 0, 200);
		playerControllerVariables.PlayerHealth = health;
	}

	public static void ReducePlayerHealth(int reduceValue, PlayerControllerVariables playerControllerVariables)
	{
		var health = playerControllerVariables.PlayerHealth;
		health -= reduceValue;
		health = health <= 0 ? 0 : Math.Clamp(health, 0, 200);
		playerControllerVariables.PlayerHealth = health;
	}

	public static void ReduceEnemyHealth(int reduceValue, int maxValue, Slime slime)
	{
		slime.Health -= reduceValue;
		slime.Health = Math.Clamp(slime.Health, slime.Health, maxValue);
	}
	
}