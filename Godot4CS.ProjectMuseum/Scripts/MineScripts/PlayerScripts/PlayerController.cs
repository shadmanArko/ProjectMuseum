using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

public partial class PlayerController : CharacterBody2D
{
	[Export] private AnimationController _animationController;

	#region Movement Variables

	[Export] private int _maxSpeed = 100;
	[Export] private int _acceleration = 20;
	[Export] private int _friction = 100;
	
	[Export] private int _deceleration = 5;
	[Export] private float _interpolationTime = 0.5f;

	#endregion

	[Export] private float _gravity;
	[Export] private float _terminalVelocity;
	[Export] private bool _isGrounded;

	public override void _PhysicsProcess(double delta)
	{
		PlayerMovement(delta);
	}
    
	private void PlayerMovement(double delta)
	{
		var input = GetInputKeyboard();
		if (input == Vector2.Zero)
		{
			if (Velocity.Length() > _friction * delta)
				Velocity -= Velocity.Normalized() * (_friction * (float)delta);
			else
				Velocity = Vector2.Zero;
		}
		else
		{
			Velocity = (input * _acceleration * (float)delta);
			Velocity = Velocity.LimitLength(_maxSpeed);
		}

		_animationController.SetAnimation(Velocity, PlayerAttack());
		ApplyGravity(delta);
		DetectCollision();
	}

	private void ApplyGravity(double delta)
	{
		if(_isGrounded) return;

		var previousGravityY = Velocity.Y;
		var newGravityY = Mathf.Round(Velocity.Y + _gravity * (float)delta);
		newGravityY = Mathf.Clamp(newGravityY, -Mathf.Inf, _terminalVelocity);
		var newVelocityY = (previousGravityY + newGravityY) * 0.5f;
		Velocity = new Vector2(Velocity.X, newVelocityY);
	}

	private void DetectCollision()
	{
		var collision = MoveAndCollide(Velocity, recoveryAsCollision: true);
	}

	#region Input

	private Vector2 GetInputKeyboard()
	{
		var motion = new Vector2
		{
			X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
			Y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up")
		};
        
		return motion.Normalized();
	}
	
	private bool PlayerAttack()
	{
		var input = Input.IsActionJustReleased("ui_left_click");
		if (input) MineActions.OnPlayerAttackAction?.Invoke();
		return input;
	}
	
	public override void _Input(InputEvent @event)
	{
		if(@event is not InputEventMouseMotion) return;
		var mousePos = GetGlobalMousePosition();
		var angle = GetAngleTo(mousePos);
		var degree = angle * (180 / Math.PI);
		MineActions.OnMouseMotionAction?.Invoke(degree);
	}

	#endregion
}