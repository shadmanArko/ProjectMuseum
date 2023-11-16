using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerController : CharacterBody2D
{
	[Export] private AnimationController _animationController;

	private PlayerControllerVariables _playerControllerVariables;

	[Export] private float _maxVerticalVelocity;

	public override void _EnterTree()
	{
		InitializeDiReferences();
	}

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}

	public override void _PhysicsProcess(double delta)
	{
        PlayerMovement(delta);
	}
    
	private void PlayerMovement(double delta)
	{
		if (_playerControllerVariables.CanMove)
		{
			var input = GetInputKeyboard();
			if (input == Vector2.Zero)
			{
				if (Velocity.Length() > PlayerControllerVariables.Friction * delta)
					Velocity -= Velocity.Normalized() * (PlayerControllerVariables.Friction * (float)delta);
				else
					Velocity = Vector2.Zero;
			}
			else
			{
				Velocity = input * _playerControllerVariables.Acceleration * (float)delta;
				Velocity = Velocity.LimitLength(PlayerControllerVariables.MaxSpeed);
			}
		}
        
		ApplyGravity();
		if(_playerControllerVariables.CanMove) PlayerGrab();
		_animationController.SetAnimation(PlayerAttack());
		DetectCollision();
		ModifyPlayerVariables();
	}

	private void ApplyGravity()
	{
		if(_playerControllerVariables.IsGrounded || _playerControllerVariables.IsHanging) return;

		var previousVerticalVelocity = Velocity.Y;
		var currentVerticalVelocity = Mathf.Clamp(previousVerticalVelocity + _playerControllerVariables.Gravity, 0, _maxVerticalVelocity);

		Velocity = new Vector2(Velocity.X, currentVerticalVelocity);
	}

	private void ModifyPlayerVariables()
	{
		_playerControllerVariables.Position = Position;
		_playerControllerVariables.Velocity = Velocity;
	}

	private void DetectCollision()
	{
		var collision = MoveAndCollide(Velocity, recoveryAsCollision: true);
		if (collision == null)
		{
			_playerControllerVariables.IsGrounded = false;
			return;
		}
        
        MineActions.OnPlayerCollisionDetection?.Invoke(collision);
	}

	#region Input

	private Vector2 GetInputKeyboard()
	{
		Vector2 motion;
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
		_playerControllerVariables.IsAttacking = input;
		return input;
	}

	private void PlayerGrab()
	{
		var grab = Input.IsActionJustReleased("toggle_grab");
		if (!grab) return;
		_playerControllerVariables.IsHanging = !_playerControllerVariables.IsHanging;
		_playerControllerVariables.Acceleration = _playerControllerVariables.IsHanging ? PlayerControllerVariables.MaxSpeed / 2 : PlayerControllerVariables.MaxSpeed;
	}
	
	public override void _Input(InputEvent @event)
	{
		if(@event is not InputEventMouseMotion) return;
		var mousePos = GetGlobalMousePosition();
		var angle = GetAngleTo(mousePos);
		var degree = angle * (180 / Math.PI);
		
		_playerControllerVariables.MouseDirection = degree switch
		{
			<= 45 and > -45 => Vector2I.Right,
			<= -45 and > -135 => Vector2I.Up,
			> 45 and <= 135 => Vector2I.Down,
			_ => Vector2I.Left
		};
		
		MineActions.OnMouseMotionAction?.Invoke(degree);
	}

	#endregion
}