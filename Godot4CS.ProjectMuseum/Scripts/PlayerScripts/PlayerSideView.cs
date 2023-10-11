using Godot;

public partial class PlayerSideView : CharacterBody2D
{
	[Export] private int _maxSpeed = 100;
	[Export] private int _acceleration = 50;
	[Export] private int _friction = 50;
	
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
			Velocity = Vector2.Zero;
		else
		{
			Velocity += input * _acceleration * (float) delta;
			Velocity = Velocity.LimitLength(_maxSpeed);
		}
        
		DetectCollision();
	}
	
	private void DetectCollision()
	{
		var collision = MoveAndCollide(Velocity);
		if(collision == null) return;
		
		Velocity = Vector2.Zero;

		var inverseDirection = collision.GetNormal();
		var collidedObject = collision.GetColliderShape();
		var inverseVelocity = inverseDirection * _deceleration;
		inverseVelocity = inverseVelocity.Lerp(inverseVelocity, _interpolationTime);
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
