using Godot;

public partial class CharacterBody2DIsometric : CharacterBody2D
{
	private const float SPEED = 100;
    private Vector2 motion = Vector2.Zero;

    // Converts any Vector2 coordinates or motion from the cartesian to the isometric system
    private Vector2 CartesianToIsometric(Vector2 cartesian)
    {
        return new Vector2(cartesian.X - cartesian.Y, (cartesian.X + cartesian.Y) / 2);
    }

    // Converts isometric coordinates back to cartesian coordinates
    private Vector2 IsometricToCartesian(Vector2 iso)
    {
        Vector2 cartPos = Vector2.Zero;
        cartPos.X = (iso.X + iso.Y * 2) / 2;
        cartPos.Y = -iso.X + cartPos.X;
        return cartPos;
    }

	public override void _PhysicsProcess(double delta)
	{
		// Everything works like you're used to in a top-down game
        Vector2 direction = Vector2.Zero;

        if (Input.IsActionPressed("move_up"))
        {
            direction += new Vector2(0, -1);
        }
        else if (Input.IsActionPressed("move_down"))
        {
            direction += new Vector2(0, 1);
        }

        if (Input.IsActionPressed("move_left"))
        {
            direction += new Vector2(-1, 0);
        }
        else if (Input.IsActionPressed("move_right"))
        {
            direction += new Vector2(1, 0);
        }

        motion = new Vector2(direction.X * (float) (SPEED * delta), direction.Y * (float)(SPEED * delta));
        // Isometric movement is movement like you're used to, converted to the isometric system
        motion = CartesianToIsometric(motion);
        MoveAndCollide(motion);
    }
	
}
