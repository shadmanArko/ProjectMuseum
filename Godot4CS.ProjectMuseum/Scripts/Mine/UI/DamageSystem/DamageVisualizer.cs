using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI.DamageSystem;

public partial class DamageVisualizer : RigidBody2D
{
	[Export] private RichTextLabel _damageValue;
	[Export] private Timer _destroyTimer;

	[Export] private Vector2 _upwardForce;

	public void SetDamageValue(int value)
	{
		_damageValue.Text = value.ToString();
		var random = new Random();
		var randomX = random.Next(-200, 200);
		var newLinerVel = new Vector2(randomX, LinearVelocity.Y);
		LinearVelocity = newLinerVel;
	}
    
	private void OnTimeOut()
	{
		GD.Print("Time out called");
		_ExitTree();
		QueueFree();
	}
    
}