using System;
using System.Diagnostics;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

public partial class PlayerController : CharacterBody2D
{
	[Export] private AnimationController _animationController;
	
	[Export] private int _maxSpeed = 100;
	[Export] private int _acceleration = 20;
	[Export] private int _friction = 100;
	
	[Export] private int _deceleration = 5;
	[Export] private float _interpolationTime = 0.5f;
    
	[Signal]
	public delegate void OnPlayerCollisionEventHandler(KinematicCollision2D collision2D);

	[Signal]
	public delegate void OnClickAttackEventHandler();

	[Signal]
	public delegate void OnMouseMotionEventHandler(double angle);

	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		PlayerMovement(delta);
	}

	private bool PlayerAttack()
	{
		var input = Input.IsActionJustReleased("ui_left_click");
		if (input)
		{
			_animationController.PlayAnimation("attack");
			EmitSignal("OnClickAttack");
		}
		return input;
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
		DetectCollision();
	}
    
	private void DetectCollision()
	{
		var collision = MoveAndCollide(Velocity, recoveryAsCollision:true);
		if(collision == null) return;
		if(PlayerAttack())
            EmitSignal("OnPlayerCollision", collision);
	}
    
	private Vector2 GetInputKeyboard()
	{
		var motion = new Vector2
		{
			X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
			Y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up")
		};
        
		return motion.Normalized();
	}
	
	public override void _Input(InputEvent @event)
	{
		if(@event is not InputEventMouseMotion) return;
		var mousePos = GetGlobalMousePosition();
		var angle = GetAngleTo(mousePos);
		var degree = angle * (180 / Math.PI);
		EmitSignal("OnMouseMotion", degree);
	}
}
