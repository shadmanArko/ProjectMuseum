using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class Guest : CharacterBody2D
{
	[Export] private float _displacementSpeed = 30;
	[Export] private float _lookAheadDistance = 20;
    private Vector2 motion = Vector2.Zero;

    [Export] private AnimationPlayer _animationPlayer;
    private AnimationPlayer _animationPlayerInstance;
    [Export] private Sprite2D _characterSprite;
    private bool _playerFacingTheFront = true;
    private double _timer = 0f;
    private double _decisionChangingInterval = 5f;
    private double _decisionChangingIntervalMin = 2f;
    private double _decisionChangingIntervalMax = 5f;
    [Export] private Array<Texture2D> _guestTextures;
    private List<MuseumTile> _listOfMuseumTile;
    private string _testString;

    private bool _canMove = false;

    public Guest()
    {
        
    }
    // Converts any Vector2 coordinates or motion from the cartesian to the isometric system
    public override void _Ready()
    {
        base._Ready();
        GetRandomInterval();
        _listOfMuseumTile = ServiceRegistry.Resolve<List<MuseumTile>>();
        GD.Print($"{Name} got {_listOfMuseumTile.Count} tiles");
        // AddToGroup("ManualSortGroup");
        _animationPlayerInstance = _animationPlayer.Duplicate() as AnimationPlayer;
        AddChild(_animationPlayerInstance);
        
        LoadRandomCharacterSprite();
        _animationPlayerInstance.Play("idle_front_facing");
    }

    

    private void GetRandomInterval()
    {
        _decisionChangingInterval = GD.RandRange(_decisionChangingIntervalMin, _decisionChangingIntervalMax);
    }

    public void Initialize(List<MuseumTile> museumTiles)
    {
        _listOfMuseumTile = museumTiles;
        _canMove = true;
        MoveLeft();
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
        if (!_canMove) return;
        _timer += delta;

        // Check if 5 seconds have passed
        if (_timer >= _decisionChangingInterval)
        {
            GetRandomInterval();
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
            if ( CheckIfNextPositionIsEmpty(nextPosition + (motion * _lookAheadDistance)))
            {
                MoveAndCollide(motion);
            }
            else
            {
                TakeNextMovementDecision();
                GD.Print(Name + "Not suitable tile");
            }
            // Move only if the next position is within the TileMap bounds
        }else if (!nextPosition.IsWorldPositionInsideTileMap(GameManager.TileMap))
        {
            TakeNextMovementDecision();
        }
    }

    private void TakeNextMovementDecision()
    {
        _timer += _decisionChangingInterval;
    }

    private Vector2I _lastCheckedPosition = new Vector2I();
    private bool _lastCheckedResult = false;
    private bool CheckIfNextPositionIsEmpty(Vector2 nextPosition)
    {
        Vector2I tilePosition = GameManager.TileMap.LocalToMap(nextPosition);
        if (_lastCheckedPosition == tilePosition)
        {
            return _lastCheckedResult;
        }

        _lastCheckedPosition = tilePosition;
        foreach (var museumTile in _listOfMuseumTile)
        {
            if (museumTile.XPosition == tilePosition.X && museumTile.YPosition == tilePosition.Y)
            {
                if (museumTile.ExhibitId == "string" || museumTile.ExhibitId == "")
                {
                    _lastCheckedResult = true;
                    return true;
                }
                break;
            }
        }

        _lastCheckedResult = false;
        return false;
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
