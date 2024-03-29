using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Plugins.AStarPathFinding;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.ProfessorScripts;

public partial class PathNavigatorCharacter: CharacterBody2D
{
    [Export] protected float _displacementSpeed = 30;
	[Export] private float _lookAheadDistance = 2;
    private Vector2 motion = Vector2.Zero;

    [Export] protected AnimationPlayer _animationPlayer;
    protected AnimationPlayer _animationPlayerInstance;
    [Export] private Sprite2D _characterSprite;
    private bool _playerFacingTheFront = true;
    private double _timer = 0f;
    private double _decisionChangingInterval = 5f;
    private double _decisionChangingIntervalMin = 2f;
    private double _decisionChangingIntervalMax = 5f;
    private string _testString;
    protected MuseumTileContainer _museumTileContainer;
    private bool _canMove = false;
    private List<Vector2I> _path; // New variable to store the path
    [Export] private int _currentPathIndex = 0;
    [Export] private Vector2I _currentTargetNode;
    [Export] private Vector2I _currentNode;
    [Export] private Vector2I _startTileCoordinate;
    [Export] private Vector2I _targetTileCoordinate;
    
    // Converts any Vector2 coordinates or motion from the cartesian to the isometric system
    public override void _Ready()
    {
        base._Ready();
        GetRandomInterval();
        
        // _listOfMuseumTile = ServiceRegistry.Resolve<List<MuseumTile>>();
        // GD.Print($"{Name} got {_museumTileContainer.MuseumTiles.Count} tiles");
        // AddToGroup("ManualSortGroup");
        _animationPlayerInstance = _animationPlayer.Duplicate() as AnimationPlayer;
        AddChild(_animationPlayerInstance);
        
        _animationPlayerInstance.Play("idle_front_facing");
    }

    protected PlayerDirectionsEnum GetPlayerDirectionsEnum()
    {
        var directions = PlayerDirectionsEnum.FrontLeft;
        if (!_playerFacingTheFront)
        {
            directions = Scale.X > 0 ? PlayerDirectionsEnum.FrontLeft : PlayerDirectionsEnum.FrontRight;
        }
        else
        {
            directions = Scale.X > 0 ? PlayerDirectionsEnum.BackRight : PlayerDirectionsEnum.BackLeft;
        }

        return directions;
    }

    private void GetRandomInterval()
    {
        _decisionChangingInterval = GD.RandRange(_decisionChangingIntervalMin, _decisionChangingIntervalMax);
    }

    // public void Initialize()
    // {
    //     SetPath();
    //     // MoveLeft();
    // }
    

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
    public void SetPath( Vector2I targetTileCoordinate)
    {
        _startTileCoordinate = GameManager.tileMap.LocalToMap(Position);
        // _targetTileCoordinate =  new Vector2I(GD.RandRange(-10, -17), GD.RandRange(-10, -19));
        _targetTileCoordinate =  targetTileCoordinate;
        if (_targetTileCoordinate != new Vector2I(1000, 1000))
        {
            var aStarPathfinding = new AStarPathfinding(_museumTileContainer.AStarNodes.GetLength(0), _museumTileContainer.AStarNodes.GetLength(1), false);
            List<Vector2I> path = aStarPathfinding.FindPath(_startTileCoordinate, _targetTileCoordinate, _museumTileContainer.AStarNodes);
            if (path == null)
            {
                //GD.Print($"Path failed from {_startTileCoordinate} to {_targetTileCoordinate}");
            }
            _path = path;
            _currentPathIndex = 0; // Start from the beginning of the path
            _canMove = true;
            MoveToNextPathNode();
        }
        else
        {
            //GD.Print($"Exhibits finished for {Name}");
        }
        
    }

    

    private async void MoveToNextPathNode()
    {
        if (_currentPathIndex < _path.Count )
        {
            _currentTargetNode = _path[_currentPathIndex];
            _direction = _currentTargetNode - GameManager.tileMap.LocalToMap(Position);
            _currentPathIndex++;
            ControlAnimation();
        }
        else
        {
            _canMove = false; // Stop moving when the path is completed
            ControlAnimation();
            MuseumActions.PathEnded?.Invoke(this);
        }
    }
    Vector2 _direction = Vector2.Zero;
    private Vector2 _offset = new( -0.5f, -0.5f);
    public override void _PhysicsProcess(double delta)
    {
        if (_canMove)
        {
            motion = new Vector2(_direction.X * (float)(_displacementSpeed * delta),
                _direction.Y * (float)(_displacementSpeed * delta));
            // Isometric movement is movement like you're used to, converted to the isometric system
            motion = CartesianToIsometric(motion);
            MoveAndCollide(motion);
            // Check if the character has reached the current path node
            Vector2 currentTargetPosition = GameManager.tileMap.MapToLocal(_currentTargetNode);
            _currentNode = GameManager.tileMap.LocalToMap(Position);
            if (Position.DistanceTo(currentTargetPosition) < 1f || _currentNode == _currentTargetNode)
            {
                MoveToNextPathNode();
            }
        
            // Check if the character has reached the last node in the path
        }
        
    }

    private void ControlAnimation()
    {
        if (_canMove)
        {
            if (_direction == new Vector2(1, 0))
            {
                MoveRight();
            }else if (_direction == new Vector2(-1, 0))
            {
                MoveLeft();
            }else if (_direction == new Vector2(0, 1))
            {
                MoveDown();
            }else if (_direction == new Vector2(0, -1))
            {
                MoveUp();
            }
            
        }
        else
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
    }

    

    private Vector2I _lastCheckedPosition = new Vector2I();
    private bool _lastCheckedResult = false;
    private bool CheckIfNextPositionIsEmpty(Vector2 nextPosition)
    {
        Vector2I tilePosition = GameManager.tileMap.LocalToMap(nextPosition);
        if (_lastCheckedPosition == tilePosition)
        {
            return _lastCheckedResult;
        }

        _lastCheckedPosition = tilePosition;
        foreach (var museumTile in _museumTileContainer.MuseumTiles)
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
        
        _characterSprite.Scale = new Vector2(1, 1);
        _animationPlayerInstance.Play("walk_forward");
        _playerFacingTheFront = true;
    }

    private void MoveLeft()
    {
        
        _characterSprite.Scale = new Vector2(1, 1);
        _animationPlayerInstance.Play("walk_backward");
        _playerFacingTheFront = false;
    }

    private void MoveDown()
    {
        
        _characterSprite.Scale = new Vector2(-1, 1);
        _animationPlayerInstance.Play("walk_forward");
        _playerFacingTheFront = true;
    }

    private void MoveUp()
    {
        
        _characterSprite.Scale = new Vector2(-1, 1);
        _animationPlayerInstance.Play("walk_backward");
        _playerFacingTheFront = false;
    }
}