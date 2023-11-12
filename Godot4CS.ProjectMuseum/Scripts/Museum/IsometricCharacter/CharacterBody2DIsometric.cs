using Godot;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;

public partial class CharacterBody2DIsometric : CharacterBody2D
{
    [Export] private float _displacementSpeed = 100;
    private Vector2 motion = Vector2.Zero;

    [Export] private AnimationPlayer _animationPlayer;

    [Export] private Sprite2D _characterSprite;

    // Converts any Vector2 coordinates or motion from the cartesian to the isometric system
    public override void _Ready()
    {
        base._Ready();
        _animationPlayer.Play("player_walk_forward");
    }

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
            _characterSprite.Scale = new Vector2(1, 1);
        }
        else if (Input.IsActionPressed("move_down"))
        {
            direction += new Vector2(0, 1);
            _characterSprite.Scale = new Vector2(-1, 1);
        }

        if (Input.IsActionPressed("move_left"))
        {
            direction += new Vector2(-1, 0);
            _characterSprite.Scale = new Vector2(-1, 1);
        }
        else if (Input.IsActionPressed("move_right"))
        {
            direction += new Vector2(1, 0);
            _characterSprite.Scale = new Vector2(1, 1);
        }

        motion = new Vector2(direction.X * (float)(_displacementSpeed * delta),
            direction.Y * (float)(_displacementSpeed * delta));
        // Isometric movement is movement like you're used to, converted to the isometric system
        motion = CartesianToIsometric(motion);
        Vector2 nextPosition = Position + motion;

        // Check if the next position is within the bounds of the TileMap
        if (IsPositionInsideTileMap(nextPosition))
        {
            // Move only if the next position is within the TileMap bounds
            MoveAndCollide(motion);
        }
    }

// Check if a given position is inside the bounds of the TileMap
    private bool IsPositionInsideTileMap(Vector2 position)
    {
        // Assuming _tileMap is a reference to your Isometric TileMap node
        Vector2 tilemapPos = GameManager.TileMap.LocalToMap(position);
    
        // Assuming tileSize is the size of your isometric tile
        Vector2 tileSize = new Vector2(-20, -24); // Replace with your actual tile size
    
        return tileSize.Y < tilemapPos.Y && tilemapPos.Y < 0 &&
               tileSize.X < tilemapPos.X && tilemapPos.X < 0;
    }
}