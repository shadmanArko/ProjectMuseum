using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.Museum.ProfessorScripts;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class CharacterBody2DIsometric : PathNavigatorCharacter
{
    private Vector2 motion = Vector2.Zero;


    [Export] private Sprite2D _characterSprite;
    private bool _playerFacingTheFront = true;

    private MuseumTileContainer _museumTileContainer;
    // Converts any Vector2 coordinates or motion from the cartesian to the isometric system
    public override async void _Ready()
    {
        base._Ready();
        // AddToGroup("ManualSortGroup");
        _animationPlayerInstance.Play("idle_front_facing");
        await Task.Delay(1000);
        _museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
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

    private Vector2I _lastPlayerTile = new Vector2I(1000, 1000);
    public override void _PhysicsProcess(double delta)
    {
        // Everything works like you're used to in a top-down game
        Vector2 direction = Vector2.Zero;

        if (Input.IsActionPressed("move_up"))
        {
            direction += new Vector2(0, -1);
            _characterSprite.Scale = new Vector2(-1, 1);
            _animationPlayerInstance.Play("walk_backward");
            _playerFacingTheFront = false;
        }
        else if (Input.IsActionPressed("move_down"))
        {
            direction += new Vector2(0, 1);
            _characterSprite.Scale = new Vector2(-1, 1);
            _animationPlayerInstance.Play("walk_forward");
            _playerFacingTheFront = true;
        }

        if (Input.IsActionPressed("move_left"))
        {
            direction += new Vector2(-1, 0);
            _characterSprite.Scale = new Vector2(1, 1);
            _animationPlayerInstance.Play("walk_backward");
            _playerFacingTheFront = false;
        }
        else if (Input.IsActionPressed("move_right"))
        {
            direction += new Vector2(1, 0);
            _characterSprite.Scale = new Vector2(1, 1);
            _animationPlayerInstance.Play("walk_forward");
            _playerFacingTheFront = true;
        }

        if (direction == Vector2.Zero)
        {
            if (_playerFacingTheFront)
            {
                _animationPlayerInstance.Play("idle_front_facing");
            }
            else
            {
                _animationPlayerInstance.Play("idle_back_facing");
            }
            return;
        }
        
        motion = new Vector2(direction.X * (float)(_displacementSpeed * delta),
            direction.Y * (float)(_displacementSpeed * delta));
        // Isometric movement is movement like you're used to, converted to the isometric system
        motion = CartesianToIsometric(motion);
        Vector2 nextPosition = Position + motion;
        
        // Check if the next position is within the bounds of the TileMap
        if (_museumTileContainer == null || _museumTileContainer.MuseumTiles.Count == 0)
        {
            return;
        }
        if (_museumTileContainer.MuseumTiles.CheckIfNextPositionIsEmpty(nextPosition) && (direction.X == 0 || direction.Y == 0))
        {
            // Move only if the next position is within the TileMap bounds
            var newTile = GameManager.TileMap.LocalToMap(Position);
            if ( newTile != _lastPlayerTile)
            {
                _lastPlayerTile = newTile;
                MuseumActions.PlayerEnteredNewTile?.Invoke(newTile);
            }
            MoveAndCollide(motion);
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("Interact"))
        {
            var tile = GameManager.TileMap.LocalToMap(Position);
            var directions = GetPlayerDirectionsEnum();
            MuseumActions.OnPlayerInteract?.Invoke(tile, directions);
        }
    }

   
}

public enum PlayerDirectionsEnum
{
    FrontRight,
    FrontLeft,
    BackRight,
    BackLeft
}