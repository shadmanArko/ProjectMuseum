using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

public partial class PlayerController : CharacterBody2D
{
	[Export] private MineGenerationController _mapGenerationController;
	[Export] private AnimationController _animationController;

	#region Movement Variables

	[Export] private int _maxSpeed = 100;
	[Export] private int _acceleration = 20;
	[Export] private int _friction = 100;
	
	[Export] private int _deceleration = 5;
	[Export] private float _interpolationTime = 0.5f;

	#endregion

	[Export] private float _gravity;
	[Export] private const float InitialGravity = 800f;
	[Export] private const float MaxGravity = 800f;
	[Export] private bool _isGrounded;

	[Export] private bool _isHanging;

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
			Velocity = input * _acceleration * (float)delta;
			Velocity = Velocity.LimitLength(_maxSpeed);
		}

		PlayerGrab();
		_animationController.SetAnimation(Velocity, PlayerAttack());
		ApplyGravity(delta);
		DetectCollision();
	}

	private void ApplyGravity(double delta)
	{
		if(_isGrounded) return;

		var previousGravityY = Velocity.Y;
		var newGravityY = Mathf.Round(Velocity.Y + _gravity * (float)delta);
		newGravityY = Mathf.Clamp(newGravityY, -Mathf.Inf, MaxGravity);
		var newVelocityY = (previousGravityY + newGravityY) * 0.5f;
		Velocity = new Vector2(Velocity.X, newVelocityY);
	}

	private void DetectCollision()
	{
		var collision = MoveAndCollide(Velocity, recoveryAsCollision: true);
		if (collision == null)
		{
			_isGrounded = false;
			return;
		}
		var tileMap = _mapGenerationController._mineGenerationView.TileMap;
		if (collision.GetCollider() == tileMap)
		{
			var tilePos = _mapGenerationController._mineGenerationView.TileMap.LocalToMap(Position);
			var playerPos = _mapGenerationController._mineGenerationView.TileMap.LocalToMap(Position);
			tilePos -= (Vector2I) collision.GetNormal();

			if (tilePos.Y > playerPos.Y)
				_isGrounded = true;
            
			GD.Print($"tilepos: {tilePos.X}, {tilePos.Y} | PlayerPos: {playerPos.X}, {playerPos.Y}");
		}
	}

	#region Input

	private Vector2 GetInputKeyboard()
	{
		var motion = new Vector2();
		if (_isHanging)
		{
			motion = new Vector2
			{
				X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
				Y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up")
			};
		}
		else
		{
			motion = new Vector2
			{
				X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left")
			};
		}
		
		return motion.Normalized();
	}
	
	private bool PlayerAttack()
	{
		var input = Input.IsActionJustReleased("ui_left_click");
		if (input) MineActions.OnPlayerAttackAction?.Invoke();
		return input;
	}

	private void PlayerGrab()
	{
		var grab = Input.IsActionJustReleased("toggle_grab");
		if (grab)
		{
			_isHanging = !_isHanging;
			_gravity = _isHanging ? 1 : InitialGravity;
			_acceleration = _isHanging ? _maxSpeed / 2 : _maxSpeed;
		}
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