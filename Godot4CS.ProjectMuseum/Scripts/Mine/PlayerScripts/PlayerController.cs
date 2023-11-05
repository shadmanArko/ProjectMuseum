using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.MineScripts;
using Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerController : CharacterBody2D
{
	[Export] private AnimationController _animationController;

	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;

	public override void _Ready()
	{
		InitializeDiReferences();
		var vectorPos = new Vector2(_mineGenerationVariables.Cells[_mineGenerationVariables.GridWidth / 2, 0].PositionX,
			_mineGenerationVariables.Cells[_mineGenerationVariables.GridWidth / 2, 0].PositionY);
		var pos = vectorPos + new Vector2(0,-15);
		Position = pos;
	}

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	public override void _PhysicsProcess(double delta)
	{
		PlayerMovement(delta);

		#region Testing Purposes

		if (Input.IsActionJustReleased("Test"))
		{
			GD.Print($"acceleration {_playerControllerVariables.Acceleration}");
			GD.Print($"max speed {_playerControllerVariables.MaxSpeed}");
			GD.Print($"friction {_playerControllerVariables.Friction}");
			GD.Print($"gravity {_playerControllerVariables.Gravity}");
			GD.Print($"initial gravity {_playerControllerVariables.InitialGravity}");
			GD.Print($"max gravity {_playerControllerVariables.MaxGravity}");
			GD.Print($"is grounded {_playerControllerVariables.IsGrounded}");
			GD.Print($"is attacking {_playerControllerVariables.IsAttacking}");
			GD.Print($"is hanging {_playerControllerVariables.IsHanging}");
		}

		#endregion
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
        
		ModifyPlayerVariables();
		PlayerGrab();
		_animationController.SetAnimation(PlayerAttack());
		DetectCollision();
		ApplyGravity(delta);
	}

	private void ModifyPlayerVariables()
	{
		_playerControllerVariables.Position = Position;
		_playerControllerVariables.Velocity = Velocity;
	}

	private void ApplyGravity(double delta)
	{
		if (_playerControllerVariables.IsGrounded) return;

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
			//GD.Print("Collision is null");
			_playerControllerVariables.IsGrounded = false;
			return;
		}
		
		//GD.Print("Collision is NOT null");
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