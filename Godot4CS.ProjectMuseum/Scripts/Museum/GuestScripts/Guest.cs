using Godot;
using System;
using System.IO;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;

public partial class Guest : CharacterBody2D
{
	[Export] private float _displacementSpeed = 100;
    private Vector2 motion = Vector2.Zero;

    [Export] private AnimationPlayer _animationPlayer;
    private AnimationPlayer _animationPlayerInstance;
    [Export] private Sprite2D _characterSprite;
    private bool _playerFacingTheFront = true;
    private double _timer = 0f;
    private float _decisionChangingInterval = 5f;
    [Export] private Array<Texture2D> _guestTextures;
    // Converts any Vector2 coordinates or motion from the cartesian to the isometric system
    public override void _Ready()
    {
        base._Ready();
        _animationPlayerInstance = _animationPlayer.Duplicate() as AnimationPlayer;
        AddChild(_animationPlayerInstance);
        
        LoadRandomCharacterSprite();
        _animationPlayerInstance.Play("idle_front_facing");
    }

    private void LoadRandomCharacterSprite()
    {
        _characterSprite.Texture = _guestTextures[GD.RandRange(0, _guestTextures.Count -1)];
        
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
    Vector2 _direction = Vector2.Zero;
    public override void _PhysicsProcess(double delta)
    {
        _timer += delta;

        // Check if 5 seconds have passed
        if (_timer >= _decisionChangingInterval)
        {
            _direction = Vector2.Zero;
            int options = GD.RandRange(1, 5);
            switch (options)
            {
                case 1 : MoveRight();
                    break;
                case 2 : MoveLeft();
                    break;
                case 3: MoveUp();
                    break;
                case 4: MoveDown();
                    break;
                case 5: _direction = Vector2.Zero;
                    break;
            }
            
            _timer = 0f;
        }
        // Everything works like you're used to in a top-down game
        
        // _direction = Vector2.Zero;

        // if (Input.IsActionPressed("move_up"))
        // {
        //     MoveUp();
        // }
        // else if (Input.IsActionPressed("move_down"))
        // {
        //     MoveDown();
        // }
        //
        // if (Input.IsActionPressed("move_left"))
        // {
        //     MoveLeft();
        // }
        // else if (Input.IsActionPressed("move_right"))
        // {
        //     MoveRight();
        // }

        if (_direction.IsEqualApprox(Vector2.Zero))
        {
            if (_playerFacingTheFront)
            {
                _animationPlayerInstance.Play("idle_front_facing");
            }
            else
            {
                _animationPlayerInstance.Play("idle_back_facing");
            }
        }
        
        motion = new Vector2(_direction.X * (float)(_displacementSpeed * delta),
            _direction.Y * (float)(_displacementSpeed * delta));
        // Isometric movement is movement like you're used to, converted to the isometric system
        motion = CartesianToIsometric(motion);
        Vector2 nextPosition = Position + motion;

        // Check if the next position is within the bounds of the TileMap
        if (nextPosition.IsWorldPositionInsideTileMap(GameManager.TileMap) && (_direction.X == 0 || _direction.Y == 0))
        {
            // Move only if the next position is within the TileMap bounds
            MoveAndCollide(motion);
        }
    }

    private void MoveRight()
    {
        _direction += new Vector2(1, 0);
        _characterSprite.Scale = new Vector2(1, 1);
        _animationPlayerInstance.Play("walk_forward");
        _playerFacingTheFront = true;
    }

    private void MoveLeft()
    {
        _direction += new Vector2(-1, 0);
        _characterSprite.Scale = new Vector2(1, 1);
        _animationPlayerInstance.Play("walk_backward");
        _playerFacingTheFront = false;
    }

    private void MoveDown()
    {
        _direction += new Vector2(0, 1);
        _characterSprite.Scale = new Vector2(-1, 1);
        _animationPlayerInstance.Play("walk_forward");
        _playerFacingTheFront = true;
    }

    private void MoveUp()
    {
        _direction += new Vector2(0, -1);
        _characterSprite.Scale = new Vector2(-1, 1);
        _animationPlayerInstance.Play("walk_backward");
        _playerFacingTheFront = false;
    }
}
