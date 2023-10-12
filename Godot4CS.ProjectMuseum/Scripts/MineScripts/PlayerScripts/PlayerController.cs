using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

public partial class PlayerController : CharacterBody2D
{
	[Export] private Sprite2D _sprite;
	[Export] private AnimationPlayer _animationPlayer;
	
	[Export] private int _maxSpeed = 100;
	[Export] private int _acceleration = 100;
	[Export] private int _friction = 50;
	[Export] private float _gravity = 200f;
	
	[Export] private int _deceleration = 5;
	[Export] private float _interpolationTime = 0.5f;

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
			var velocity  = Velocity + input * _acceleration * (float) delta;
			velocity = velocity.LimitLength(_maxSpeed);
			Velocity = velocity;
		}
		
		
		//AddGravity(delta);
        SetAnimation();
		//DetectCollision();
	}

	private void AddGravity(double delta)
	{
		var tempVelocity = Velocity;
		tempVelocity.Y += (float)delta * _gravity;
		Velocity = tempVelocity;
	}

	private void SetAnimation()
	{
		var tempVelocity = Velocity.Normalized();
		if(tempVelocity.X == 0)
			PlayAnimation("idle");
		else
		{
			switch (tempVelocity.X)
			{
				case > 0:
					_sprite.FlipH = false;
					PlayAnimation("run");
					break;
				case < 0:
					_sprite.FlipH = true;
					PlayAnimation("run");
					break;
				default:
					_sprite.FlipH = _sprite.FlipH;
					break;
			}

			
		}
	}

	private void PlayAnimation(string state)
	{
		_animationPlayer.Play(state);
	}
	
	private void DetectCollision()
	{
		var collision = MoveAndCollide(Velocity);
		if(collision == null) return;
        
		var inverseDirection = collision.GetNormal();
		var collidedObject = collision.GetColliderShape();
		var inverseVelocity = inverseDirection * _deceleration;
		inverseVelocity = inverseVelocity.Lerp(inverseVelocity, _interpolationTime);
		Velocity = Vector2.Zero;
		Velocity = inverseVelocity;

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
}