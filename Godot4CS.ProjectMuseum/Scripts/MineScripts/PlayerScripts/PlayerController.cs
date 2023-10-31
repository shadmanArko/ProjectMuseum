using System;
using System.Diagnostics;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

public partial class PlayerController : CharacterBody2D
{
	[Export] private MineGenerationController _mapGenerationController;	//todo: has to be removed from here
	[Export] private AnimationController _animationController;

	private PlayerControllerVariables _playerControllerVariables;
	
	// #region Movement Variables
	//
	// [Export] private int _maxSpeed = 100;
	// [Export] private int _acceleration = 100;
	// [Export] private int _friction = 200;
	//
	// [Export] private int _deceleration = 2;
	// [Export] private float _interpolationTime = 0.5f;
	//
	// #endregion
	//
	// #region Gravity Variables
	//
	// [Export] private float _gravity;
	// [Export] private const float InitialGravity = 800f;
	// [Export] private const float MaxGravity = 800f;
	// [Export] private bool _isGrounded;
	//
	// [Export] private bool _isHanging;
	//
	// #endregion
    

	public override void _Ready()
	{
		base._Ready();
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}

	public override void _PhysicsProcess(double delta)
	{
		PlayerMovement(delta);
		GD.Print($"acceleration {_playerControllerVariables.Acceleration}");
		GD.Print($"max speed {_playerControllerVariables.MaxSpeed}");
	}
    
	private void PlayerMovement(double delta)
	{
		var input = GetInputKeyboard();
		if (input == Vector2.Zero)
		{
			if (Velocity.Length() > _playerControllerVariables.Friction * delta)
				Velocity -= Velocity.Normalized() * (_playerControllerVariables.Friction * (float)delta);
			else
				Velocity = Vector2.Zero;
		}
		else
		{
			Velocity = input * _playerControllerVariables.Acceleration * (float)delta;
			Velocity = Velocity.LimitLength(_playerControllerVariables.MaxSpeed);
		}

		PlayerGrab();
		_animationController.SetAnimation(Velocity, PlayerAttack());
		ApplyGravity(delta);
		DetectCollision();
	}

	private void ApplyGravity(double delta)
	{
		if(_playerControllerVariables.IsGrounded) return;

		var previousGravityY = Velocity.Y;
		var newGravityY = Mathf.Round(Velocity.Y + _playerControllerVariables.Gravity * (float)delta);
		newGravityY = Mathf.Clamp(newGravityY, -Mathf.Inf, _playerControllerVariables.MaxGravity);
		var newVelocityY = (previousGravityY + newGravityY) * 0.5f;
		Velocity = new Vector2(Velocity.X, newVelocityY);
	}

	private void DetectCollision()
	{
		var collision = MoveAndCollide(Velocity, recoveryAsCollision: true);
		if (collision == null)
		{
			_playerControllerVariables.IsGrounded = false;
			return;
		}
		var tileMap = _mapGenerationController._mineGenerationView.TileMap;
		if (collision.GetCollider() == tileMap)
		{
			var tilePos = _mapGenerationController._mineGenerationView.TileMap.LocalToMap(Position);
			var playerPos = _mapGenerationController._mineGenerationView.TileMap.LocalToMap(Position);
			tilePos -= (Vector2I) collision.GetNormal();

			if (tilePos.Y > playerPos.Y)
				_playerControllerVariables.IsGrounded = true;
            
			GD.Print($"tilepos: {tilePos.X}, {tilePos.Y} | PlayerPos: {playerPos.X}, {playerPos.Y}");
		}
	}

	#region Input

	private Vector2 GetInputKeyboard()
	{
		var motion = new Vector2();
		if (_playerControllerVariables.IsHanging)
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
			_playerControllerVariables.IsHanging = !_playerControllerVariables.IsHanging;
			_playerControllerVariables.Gravity = _playerControllerVariables.IsHanging ? 1 : _playerControllerVariables.InitialGravity;
			_playerControllerVariables.Acceleration = _playerControllerVariables.IsHanging ? _playerControllerVariables.MaxSpeed / 2 : _playerControllerVariables.MaxSpeed;
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