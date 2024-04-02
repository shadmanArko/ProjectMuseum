using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;
using Timer = System.Threading.Timer;

namespace Godot4CS.ProjectMuseum.Scripts.Player.Systems;

public class HealthSystem
{
	private static Timer timer;
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

	public static void EffectPlayerHealth(ConsumableStatEffect statEffect, PlayerControllerVariables playerControllerVariables)
	{
		switch (statEffect.AdditiveMod)
		{
			case "FlatAdditive":
				var health = playerControllerVariables.PlayerHealth;
				health += statEffect.EffectAmount;
				health = health <= 0 ? 0 : Math.Clamp(health, 0, 200);
				playerControllerVariables.PlayerHealth = health;
				break;
				case "StaggeredAdditive":
					DateTime endTime = DateTime.Now.AddSeconds(statEffect.EffectDuration);
					var intervalInSeconds = 1;
					GD.Print("before declaring new timer");
					timer = new Timer(state =>
					{
						GD.Print("Inside timer ");
						
						if (DateTime.Now >= endTime)
						{
							// Stop the timer
							timer.Dispose();
							GD.Print("End time reached. Timer stopped.");
							return;
						}
            
						// Call your function here
						var effectRate = Mathf.CeilToInt(statEffect.EffectAmount / statEffect.EffectDuration);
						RestorePlayerHealth(effectRate, playerControllerVariables);
						GD.Print($"restoring health {playerControllerVariables.PlayerHealth}");
					}, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalInSeconds));

					// Wait for the timer to finish
					Console.WriteLine("waiting for time to finish");
					break;
		}
	}

	public static void ReduceEnemyHealth(int reduceValue, int maxValue, Slime slime)
	{
		slime.Health -= reduceValue;
		slime.Health = Math.Clamp(slime.Health, slime.Health, maxValue);
	}
	
}