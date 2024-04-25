using System;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine;
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

	private static bool _effectInProgress;
	public static async void EffectPlayerHealth(ConsumableStatEffect statEffect, PlayerControllerVariables playerControllerVariables)
	{
		if (_effectInProgress)
		{
			ReferenceStorage.Instance.MinePopUp.ShowPopUp("Health generation in progress");
			return;
		}
		_effectInProgress = true;
		
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
			RestorePlayerHealth(effectRate, playerControllerVariables);
		}

		_effectInProgress = false;
	}

	public static void ReduceEnemyHealth(int reduceValue, int maxValue, Slime slime)
	{
		slime.Health -= reduceValue;
		slime.Health = Math.Clamp(slime.Health, slime.Health, maxValue);
	}
	
}