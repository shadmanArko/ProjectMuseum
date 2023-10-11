using Godot;

public partial class PlayerSideView : CharacterBody2D
{
	[Export] public int _maxSpeed = 100;
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
		{
			// if (Velocity.Length() > _friction * delta)
			// 	Velocity -= Velocity.Normalized() * (_friction * (float)delta);
			// else
			// 	Velocity = Vector2.Zero;
            Velocity = Vector2.Zero;
		}
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
<<<<<<< Updated upstream
		
		GD.Print($"After collision velocity = {Velocity}");
=======
        
		//GD.Print($"After collision velocity = {Velocity}");
>>>>>>> Stashed changes
		var inverseDirection = collision.GetNormal();
		var collidedObject = collision.GetColliderShape();
		var inverseVelocity = inverseDirection * _deceleration;
		inverseVelocity = inverseVelocity.Lerp(inverseVelocity, _interpolationTime);
		Velocity = inverseVelocity;
<<<<<<< Updated upstream
		GD.Print($"Inverse velocity {Velocity}");
		
		// var cell = collision.GetScript();
		// if(cell.Obj == null)
		// 	GD.Print("Cell is null");
		// else
		// {
		// 	var newCell = (Cell)cell.Obj;
		// 	GD.Print($"Cell position: {newCell.Pos}, breakStrength: {newCell.BreakStrength}");
		// }
=======
		//GD.Print($"Inverse velocity {Velocity}");
>>>>>>> Stashed changes
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
