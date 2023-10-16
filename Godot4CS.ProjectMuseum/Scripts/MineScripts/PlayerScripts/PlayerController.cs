using Godot;

public partial class PlayerController : CharacterBody2D
{
	[Export] private Sprite2D _sprite;
	[Export] private AnimationPlayer _animationPlayer;
	
	[Export] private int _maxSpeed = 100;
	[Export] private int _acceleration = 20;
	[Export] private int _friction = 100;
	
	[Export] private int _deceleration = 5;
	[Export] private float _interpolationTime = 0.5f;
    
	[Signal]
	public delegate void OnPlayerCollisionEventHandler(KinematicCollision2D collision2D);

	public override void _PhysicsProcess(double delta)
	{
		PlayerMovement(delta);
	}

	private bool PlayerAttack()
	{
		var input = Input.IsActionJustReleased("attack");
		if(input) PlayAnimation("attack");
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
			Velocity += (input * _acceleration * (float)delta);
			Velocity = Velocity.LimitLength(_maxSpeed);
		}

		SetAnimation();
		DetectCollision();
	}
	
	private void SetAnimation()
	{
		var tempVelocity = Velocity.Normalized();
		if(tempVelocity.X == 0 && !PlayerAttack())
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
}
