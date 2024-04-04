using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Plugins.AStarPathFinding;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.GuestScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

public partial class Guest : GuestAi
{
	[Export] private float _minDisplacementSpeed = 20;
	[Export] private float _maxDisplacementSpeed = 30;
	private float _displacementSpeed = 30;
	[Export] private float _lookAheadDistance = 2;
    private Vector2 motion = Vector2.Zero;
    [Export] private Vector2I _museumGateTile = new (0, -2);
    [Export] private AnimationPlayer _animationPlayer;
    private AnimationPlayer _animationPlayerInstance;
    [Export] private Node2D _characterPartsParent;
    [Export] private Sprite2D _characterShadow;
    [Export] private Sprite2D _characterSkin;
    [Export] private Sprite2D _characterEye;
    [Export] private Sprite2D _characterHair;
    [Export] private Sprite2D _characterShoe;
    [Export] private Sprite2D _characterPant;
    [Export] private Sprite2D _characterShirt;
    [Export] private Sprite2D _characterOverCloth;
    [Export] private Array<Texture2D> _shadowTextures;
    [Export] private Array<Texture2D> _skinTextures;
    [Export] private Array<Texture2D> _eyeTextures;
    [Export] private Array<Texture2D> _hairTextures;
    [Export] private Array<Texture2D> _shoeTextures;
    [Export] private Array<Texture2D> _pantTextures;
    [Export] private Array<Texture2D> _shirtTextures;
    [Export] private Array<Texture2D> _overClothTextures;

    private bool _playerFacingTheFront = true;
    private double _timer = 0f;
    private double _decisionChangingInterval = 5f;
    private double _decisionChangingIntervalMin = 2f;
    private double _decisionChangingIntervalMax = 5f;
    private string _testString;
    private MuseumTileContainer _museumTileContainer;
    private bool _canMove = false;
    private List<Vector2I> _path; // New variable to store the path
    [Export] private int _currentPathIndex = 0;
    [Export] private Vector2I _currentTargetNode;
    [Export] private Vector2I _currentNode;
    [Export] private Vector2I _startTileCoordinate;
    [Export] private Vector2I _targetTileCoordinate;
    [Export] private int _currentExhibitIndex = 0;
    private bool _gamePaused = false;
    private bool _insideMuseum = false;
    private bool _wantsToEnterMuseum = false;
    private List<Vector2I> _listOfSceneExitPoints;
    public Guest()
    {
        
    }
    // Converts any Vector2 coordinates or motion from the cartesian to the isometric system
    public override void _Ready()
    {
        base._Ready();
        _museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
        MuseumActions.OnTimePauseValueUpdated += OnTimePauseValueUpdated;
        GetRandomInterval();
        
        // _listOfMuseumTile = ServiceRegistry.Resolve<List<MuseumTile>>();
        //GD.Print($"{Name} got {_museumTileContainer.MuseumTiles.Count} tiles");
        // AddToGroup("ManualSortGroup");
        _animationPlayerInstance = _animationPlayer.Duplicate() as AnimationPlayer;
        AddChild(_animationPlayerInstance);
        
        LoadRandomCharacterSprite();
        _animationPlayerInstance.Play("idle_front_facing");
    }
    public void Initialize(GuestBuildingParameter guestBuildingParameter, List<Vector2I> sceneExitPoints)
    {
        availableMoney = guestBuildingParameter.GuestMoneyRange.GetRandom();
        hungerLevel = guestBuildingParameter.HungerLevelRange.GetRandom();
        thirstLevel = guestBuildingParameter.ThirstLevelRange.GetRandom();
        chargeLevel = guestBuildingParameter.ChargeLevelRange.GetRandom();
        interestInArtifactLevel = guestBuildingParameter.InterestInArtifactLevelRange.GetRandom();
        entertainmentLevel = guestBuildingParameter.EntertainmentLevelRange.GetRandom();
        bladderLevel = guestBuildingParameter.BladderLevelRange.GetRandom();
        energyLevel = guestBuildingParameter.EnergyLevelRange.GetRandom();
        
        hungerDecayRate = guestBuildingParameter.HungerDecayRange.GetRandom();
        thirstDecayRate = guestBuildingParameter.ThirstDecayRange.GetRandom();
        chargeDecayRate = guestBuildingParameter.ChargeDecayRange.GetRandom();
        interestInArtifactDecayRate = guestBuildingParameter.InterestInArtifactDecayRange.GetRandom();
        entertainmentDecayRate = guestBuildingParameter.EntertainmentDecayRange.GetRandom();
        bladderDecayRate = guestBuildingParameter.BladderDecayRange.GetRandom();
        energyDecayRate = guestBuildingParameter.EnergyDecayRange.GetRandom();
        _listOfSceneExitPoints = sceneExitPoints;
        _displacementSpeed =  (float)GD.RandRange(_minDisplacementSpeed, _maxDisplacementSpeed);
        SetPath();
        // GD.Print($"{Name} food: {hungerLevel}, drink: {thirstLevel}, charge: {chargeLevel}");
        // MoveLeft();
    }
    private void OnTimePauseValueUpdated(bool obj)
    {
        _gamePaused = obj;
        ControlAnimation();
    }


    private void GetRandomInterval()
    {
        _decisionChangingInterval = GD.RandRange(_decisionChangingIntervalMin, _decisionChangingIntervalMax);
    }

    
    private void LoadRandomCharacterSprite()
    {
        _characterShadow.Texture = _shadowTextures[GD.RandRange(0, _shadowTextures.Count -1)];
        _characterSkin.Texture = _skinTextures[GD.RandRange(0, _skinTextures.Count -1)];
        _characterEye.Texture = _eyeTextures[GD.RandRange(0, _eyeTextures.Count -1)];
        _characterHair.Texture = _hairTextures[GD.RandRange(0, _hairTextures.Count -1)];
        _characterShoe.Texture = _shoeTextures[GD.RandRange(0, _shoeTextures.Count -1)];
        _characterPant.Texture = _pantTextures[GD.RandRange(0, _pantTextures.Count -1)];
        _characterShirt.Texture = _shirtTextures[GD.RandRange(0, _shirtTextures.Count -1)];
        _characterOverCloth.Texture = _overClothTextures[GD.RandRange(0, _overClothTextures.Count -1)];
        
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

    private GuestNeedsEnum currentNeed;
    public void SetPath()
    {
        _startTileCoordinate = GameManager.tileMap.LocalToMap(Position);
        if (_insideMuseum)
        {
            // _targetTileCoordinate =  new Vector2I(GD.RandRange(-10, -17), GD.RandRange(-10, -19));
            currentNeed = CheckForNeedsToFulfill();
            if (currentNeed == GuestNeedsEnum.Hunger || currentNeed == GuestNeedsEnum.Thirst)
            {
                _targetTileCoordinate =  GetClosestShopLocation();
            }
            else if (currentNeed == GuestNeedsEnum.Bladder)
            {
                _targetTileCoordinate =  GetClosestWashroomLocation();
            }else
            {
                _targetTileCoordinate =  GetTargetExhibitViewingLocation();
            }
            
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
                if (!_exitingMuseum)
                {
                    //GD.Print($"Exhibits finished for {Name}");
                    ExitMuseum();
                }
            
            }
        }
        else
        {
            var probability = GD.RandRange(0, 100);
            if (probability < 20 && !_exitingMuseum && GameManager.isMuseumGateOpen)
            {
                _targetTileCoordinate =  _museumGateTile;
                _wantsToEnterMuseum = true;
            }
            else
            {
                _targetTileCoordinate =  GetTargetTargetTileCoordinateOutsideMuseum();
            }
            var aStarPathfinding = new AStarPathfinding(GameManager.outSideMuseumNodes.GetLength(0), GameManager.outSideMuseumNodes.GetLength(1), false);
            List<Vector2I> path = aStarPathfinding.FindPath(_startTileCoordinate, _targetTileCoordinate, GameManager.outSideMuseumNodes);
            if (path == null)
            {
                GD.PrintErr($"Path failed from {_startTileCoordinate} to {_targetTileCoordinate}");
            }
            _path = path;
            _currentPathIndex = 0; // Start from the beginning of the path
            _canMove = true;
            MoveToNextPathNode();
        }
        
    }

    private Vector2I GetClosestShopLocation()
    {
        if (_museumTileContainer.DecorationShops.Count > 0)
        {
            _startTileCoordinate = GameManager.tileMap.LocalToMap(Position);
            var shop = _museumTileContainer.DecorationShops.GetClosestShopToLocation(_startTileCoordinate);
            Vector2I coordinate = _museumTileContainer.MuseumTiles.GetClosestEmptyTileToCoordinate(new Vector2I(shop.XPosition, shop.YPosition));
            GD.Print($"Found closest shop coordinate {coordinate}");
            return coordinate;
        }
    
    
        return new Vector2I(1000, 1000);
        
    }
    private Vector2I GetClosestWashroomLocation()
    {
        if (_museumTileContainer.Sanitations.Count > 0)
        {
            _startTileCoordinate = GameManager.tileMap.LocalToMap(Position);
            var washroomToLocation = _museumTileContainer.Sanitations.GetClosestWashroomToLocation(_startTileCoordinate);
            Vector2I coordinate = _museumTileContainer.MuseumTiles.GetClosestEmptyTileToCoordinate(new Vector2I(washroomToLocation.XPosition, washroomToLocation.YPosition));
            GD.Print($"Found closest washroom coordinate {coordinate}");
            return coordinate;
        }
    
    
        return new Vector2I(1000, 1000);
        
    }

    private Vector2I GetTargetTargetTileCoordinateOutsideMuseum()
    {
        var target = _listOfSceneExitPoints[GD.RandRange(0, _listOfSceneExitPoints.Count-1)];
        while (target.DistanceTo(_startTileCoordinate) < 5)
        {
            target = _listOfSceneExitPoints[GD.RandRange(0, _listOfSceneExitPoints.Count-1)];
        }

        return target;
    }

    private bool _exitingMuseum = false;
    private void ExitMuseum()
    {
        _exitingMuseum = true;
        _startTileCoordinate = GameManager.tileMap.LocalToMap(Position);
        // _targetTileCoordinate =  new Vector2I(GD.RandRange(-10, -17), GD.RandRange(-10, -19));
        _targetTileCoordinate = _museumGateTile;
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
    }

    private Vector2I GetTargetExhibitViewingLocation()
    {
        if (_currentExhibitIndex < _museumTileContainer.Exhibits.Count)
        {
            var exhibit = _museumTileContainer.Exhibits[_currentExhibitIndex];
            Vector2I coordinate = _museumTileContainer.MuseumTiles.GetClosestEmptyTileToExhibit(exhibit);
            //GD.Print($"Found closest coordinate {coordinate}");
            _currentExhibitIndex++;
            return coordinate;
        }
        else
        {
            return new Vector2I(1000, 1000);
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
        else if (!_insideMuseum)
        {
            // Visible = false;
            // await Task.Delay((int) (GD.RandRange(_decisionChangingIntervalMin,_decisionChangingIntervalMax)*1000));
            // Visible = true;
            // SetPath();
            if (_wantsToEnterMuseum)
            {
                if (GameManager.isMuseumGateOpen)
                {
                    _insideMuseum = true;
                    MuseumActions.OnGuestEnterMuseum?.Invoke();
                    _canMove = true;
                    SetPath();
                }
                else
                {
                    _wantsToEnterMuseum = false;
                    SetPath();
                }
            }
            else
            {
                _canMove = false;
                MuseumActions.OnGuestExitScene?.Invoke();
                QueueFree(); 
            }
            
        }
        else
        {
            if (_exitingMuseum)
            {
                // _canMove = false;
                MuseumActions.OnGuestExitMuseum?.Invoke();
                // QueueFree();
                _insideMuseum = false;
                _wantsToEnterMuseum = false;
                SetPath();
                _exitingMuseum = false;
            }
            else
            {
                //Watching exhibit
                if (currentNeed == GuestNeedsEnum.Bladder)
                {
                    FillNeed(currentNeed, -64);
                }else if (currentNeed == GuestNeedsEnum.Hunger || currentNeed == GuestNeedsEnum.Thirst)
                {
                    FillNeed(currentNeed, -32);
                    _animationPlayerInstance.Play("use_back");
                }

                if (currentNeed == GuestNeedsEnum.InterestInArtifact)
                {
                    FillNeed(currentNeed, -4);
                }else FillNeed(currentNeed, -100);
                _canMove = false; // Stop moving when the path is completed
                ControlAnimation();
                await Task.Delay((int) (GD.RandRange(_decisionChangingIntervalMin,_decisionChangingIntervalMax)*1000));
                SetPath();
            }
            
        }
    }
    Vector2 _direction = Vector2.Zero;
    private Vector2 _offset = new( -0.5f, -0.5f);
    public override void _PhysicsProcess(double delta)
    {
        if (_canMove && !_gamePaused)
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
        if (_gamePaused)
        {
            if (_playerFacingTheFront)
            {
                _animationPlayerInstance.Play("RESET");
            }
            else
            {
                _animationPlayerInstance.Play("RESET_BACK");
            }
            return;
        }
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

    private void TakeNextMovementDecision()
    {
        _timer += _decisionChangingInterval;
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
        
       _characterPartsParent.Scale = new Vector2(1, 1);
        _animationPlayerInstance.Play("walk_forward");
        _playerFacingTheFront = true;
    }

    private void MoveLeft()
    {
        
       _characterPartsParent.Scale = new Vector2(1, 1);
        _animationPlayerInstance.Play("walk_backward");
        _playerFacingTheFront = false;
    }

    private void MoveDown()
    {
        
       _characterPartsParent.Scale = new Vector2(-1, 1);
        _animationPlayerInstance.Play("walk_forward");
        _playerFacingTheFront = true;
    }

    private void MoveUp()
    {
        
       _characterPartsParent.Scale = new Vector2(-1, 1);
        _animationPlayerInstance.Play("walk_backward");
        _playerFacingTheFront = false;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MuseumActions.OnTimePauseValueUpdated -= OnTimePauseValueUpdated;
    }
}
