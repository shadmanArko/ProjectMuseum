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
    private MuseumRunningDataContainer _museumRunningDataContainer;
    private bool _canMove = false;
    private List<Vector2I> _path; // New variable to store the path
    [Export] private int _currentPathIndex = 0;
    [Export] private Vector2I _currentTargetNode;
    [Export] private Vector2I _currentNode;
    [Export] private Vector2I _startTileCoordinate;
    [Export] private Vector2I _targetTileCoordinate;
    [Export] private int _currentExhibitIndex = 0;
    private bool _gamePaused = false;
    private bool _wantsToEnterMuseum = false;
    private List<Vector2I> _listOfSceneExitPoints;
    private bool _usingWashroom;
    private bool _usingShop;
    private Vector2I _currentViewingObjectOrigin;
    public Guest()
    {
        
    }
    // Converts any Vector2 coordinates or motion from the cartesian to the isometric system
    public override void _Ready()
    {
        base._Ready();
        _museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
        MuseumActions.OnTimePauseValueUpdated += OnTimePauseValueUpdated;
        MuseumActions.OnClickGuestAi += OnClickGuestAi;
        GetRandomInterval();
        
        // _listOfMuseumTile = ServiceRegistry.Resolve<List<MuseumTile>>();
        //GD.Print($"{Name} got {_museumTileContainer.MuseumTiles.Count} tiles");
        // AddToGroup("ManualSortGroup");
        _animationPlayerInstance = _animationPlayer.Duplicate() as AnimationPlayer;
        AddChild(_animationPlayerInstance);
        
        LoadRandomCharacterSprite();
        _animationPlayerInstance.Play("idle_front_facing");
    }

    private void OnClickGuestAi(GuestAi obj)
    {
        if (obj == this)
        {
            GD.Print($"Clicked Guest target {_targetTileCoordinate}, need {currentNeed}");
        }
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

        var numberOfTags = guestBuildingParameter.NumberOfInterestedTags.GetRandom();
        for (int i = 0; i < numberOfTags; i++)
        {
            var rand = GD.RandRange(1, 4);
            var tagsPool = new List<string>();
            switch (rand)
            {
                case 1:
                    tagsPool = guestBuildingParameter.ArtifactEraTags;
                    break;
                case 2:
                    tagsPool = guestBuildingParameter.ArtifactRegionTags;
                    break;
                case 3:
                    tagsPool = guestBuildingParameter.ArtifactObjectTags;
                    break;
                case 4:
                    tagsPool = guestBuildingParameter.ArtifactMaterialTags;
                    break;
                default:
                    break;
            }

            var tag = tagsPool.Shuffle()[0];
            if (interestedInTags.Contains(tag))
            {
                i--;
                continue;
            }
            interestedInTags.Add(tag);
        }
        
        
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
        if (insideMuseum)
        {
            // _targetTileCoordinate =  new Vector2I(GD.RandRange(-10, -17), GD.RandRange(-10, -19));
            currentNeed = CheckForNeedsToFulfill();
            if (currentNeed == GuestNeedsEnum.Hunger || currentNeed == GuestNeedsEnum.Thirst)
            {
                _targetTileCoordinate =  GetClosestShopLocationNew();
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
                var aStarPathfinding = new AStarPathfinding( false);
                List<Vector2I> path = aStarPathfinding.FindPath(_startTileCoordinate, _targetTileCoordinate, _museumRunningDataContainer.AStarNodes);
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
            var aStarPathfinding = new AStarPathfinding(false);
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

    private Shop _targetShop;
    private Product _targetProduct;
    private Vector2I GetClosestShopLocationNew()
    {
        _shopManager.MakeDecisionForFulfillingNeed(out var product, out var shop, this, currentNeed);
        if (shop!= null && product !=null)
        {
            _targetShop = shop;
            _targetProduct = product;
            Vector2I coordinate = Vector2I.Zero;
            _currentViewingObjectOrigin = new Vector2I(shop.XPosition, shop.YPosition);
            if (shop.RotationFrame == 0)
            {
                coordinate = new Vector2I(shop.XPosition +1, shop.YPosition);
            }else if (shop.RotationFrame == 1)
            {
                coordinate = new Vector2I(shop.XPosition , shop.YPosition +1);
            }else if (shop.RotationFrame == 2)
            {
                coordinate = new Vector2I(shop.XPosition - 1 , shop.YPosition);
            }else if (shop.RotationFrame == 3)
            {
                coordinate = new Vector2I(shop.XPosition  , shop.YPosition - 1);
            }
            return coordinate;

        }
        GD.PrintErr($"Could not find shop or product for need {currentNeed}");
        return new Vector2I(1000, 1000);

    }

    private Vector2I GetClosestShopLocation()
    {
        if (_museumRunningDataContainer.Shops.Count > 0)
        {
            _startTileCoordinate = GameManager.tileMap.LocalToMap(Position);
            var shop = _museumRunningDataContainer.Shops.GetClosestShopToLocation(_startTileCoordinate);
            _currentViewingObjectOrigin = new Vector2I(shop.XPosition, shop.YPosition);
            Vector2I coordinate = Vector2I.Zero;
            if (shop.RotationFrame == 0)
            {
                coordinate = new Vector2I(shop.XPosition +1, shop.YPosition);
            }else if (shop.RotationFrame == 1)
            {
                coordinate = new Vector2I(shop.XPosition , shop.YPosition +1);
            }else if (shop.RotationFrame == 2)
            {
                coordinate = new Vector2I(shop.XPosition - 1 , shop.YPosition);
            }else if (shop.RotationFrame == 3)
            {
                coordinate = new Vector2I(shop.XPosition  , shop.YPosition - 1);
            }
            
            GD.Print($"Found closest shop coordinate {coordinate}");
            return coordinate;
        }
    
    
        return new Vector2I(1000, 1000);
        
    }
    private Vector2I GetClosestWashroomLocation()
    {
        if (_museumRunningDataContainer.Sanitations.Count > 0)
        {
            _startTileCoordinate = GameManager.tileMap.LocalToMap(Position);
            var washroomToLocation = _museumRunningDataContainer.Sanitations.GetClosestWashroomToLocation(_startTileCoordinate);
            _currentViewingObjectOrigin = new Vector2I(washroomToLocation.XPosition, washroomToLocation.YPosition);
            Vector2I coordinate = _museumRunningDataContainer.MuseumTiles.GetClosestEmptyTileToCoordinate(new Vector2I(washroomToLocation.XPosition, washroomToLocation.YPosition));
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
            var aStarPathfinding = new AStarPathfinding(false);
            List<Vector2I> path = aStarPathfinding.FindPath(_startTileCoordinate, _targetTileCoordinate, _museumRunningDataContainer.AStarNodes);
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
        if (_currentExhibitIndex < _museumRunningDataContainer.Exhibits.Count)
        {
            var exhibit = _museumRunningDataContainer.Exhibits[_currentExhibitIndex];
            _currentViewingObjectOrigin = new Vector2I(exhibit.XPosition, exhibit.YPosition);
            TileHelpers.TargetWthOrigin coordinate = _museumRunningDataContainer.MuseumTiles.GetRandomEmptyTileClosestToExhibit(exhibit);
            GD.Print($"Found Exhibit viewing coordinate {coordinate.target} origin {coordinate.origin}");
            _currentViewingObjectOrigin = coordinate.origin;
            _currentExhibitIndex++;
            return coordinate.target;
        }
        else
        {
            return new Vector2I(1000, 1000);
        }
    }
    private async void MoveToNextPathNode()
    {
        if (_path == null)
        {
            return;
        }
        if (_currentPathIndex < _path.Count )
        {
            _currentTargetNode = _path[_currentPathIndex];
            _direction = _currentTargetNode - GameManager.tileMap.LocalToMap(Position);
            _currentPathIndex++;
            ControlAnimation();
        }
        else if (!insideMuseum)
        {
            // Visible = false;
            // await Task.Delay((int) (GD.RandRange(_decisionChangingIntervalMin,_decisionChangingIntervalMax)*1000));
            // Visible = true;
            // SetPath();
            if (_wantsToEnterMuseum)
            {
                if (GameManager.isMuseumGateOpen)
                {
                    insideMuseum = true;
                    MuseumActions.OnGuestEnterMuseum?.Invoke(this);
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
                MuseumActions.OnGuestExitMuseum?.Invoke(this);
                // QueueFree();
                insideMuseum = false;
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
                    _usingWashroom = true;

                }else if (currentNeed == GuestNeedsEnum.Hunger || currentNeed == GuestNeedsEnum.Thirst)
                {
                    FillNeed(currentNeed, -32);
                    _usingShop = true;
                }

                if (currentNeed == GuestNeedsEnum.InterestInArtifact)
                {
                    FillNeed(currentNeed, -4);
                }else FillNeed(GuestNeedsEnum.InterestInArtifact, -4);
                _canMove = false; // Stop moving when the path is completed
                ControlAnimation();
                await Task.Delay((int) (GD.RandRange(_decisionChangingIntervalMin,_decisionChangingIntervalMax)*1000));
                _usingShop = false;
                _usingWashroom = false;
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
            var offset = new Vector2(currentTargetPosition.X < Position.X ? -16 : 16,
                currentTargetPosition.Y < Position.Y ? 8 : -8);
            //currentTargetPosition += offset;
            _currentNode = GameManager.tileMap.LocalToMap(Position);
            if (Position.DistanceTo(currentTargetPosition ) < 1f && _currentNode == _currentTargetNode)
            {
                MoveToNextPathNode();
            }
        
            // Check if the character has reached the last node in the path
        }
        
    }

    private async void ControlAnimation()
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
            MoveBasedOnDirection(_direction);
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

        if (!_canMove)
        {
            if (_currentViewingObjectOrigin.X < _currentNode.X)
            {
                IdleLeft();
            }
            if (_currentViewingObjectOrigin.X > _currentNode.X)
            {
                IdleRight();
            }
            if (_currentViewingObjectOrigin.Y > _currentNode.Y)
            {
                IdleDown();
            }
            if (_currentViewingObjectOrigin.Y < _currentNode.Y)
            {
                IdleUp();
            }
            
        }
        
        if (_usingShop)
        {
            BuyProduct();
            _animationPlayerInstance.Play(_playerFacingTheFront? "use_front":"use_back");
            await Task.Delay(600);
            _animationPlayerInstance.Play(_playerFacingTheFront? "consume_front":"consume_back");

        }

        Visible = !_usingWashroom;
    }

    private void BuyProduct()
    {
        _shopManager.SellProduct(_targetProduct);
        availableMoney -= _targetProduct.BasePrice;
    }

    private void MoveBasedOnDirection(Vector2 direction)
    {
        if (direction == new Vector2(1, 0))
        {
            MoveRight();
        }
        else if (direction == new Vector2(-1, 0))
        {
            MoveLeft();
        }
        else if (direction == new Vector2(0, 1))
        {
            MoveDown();
        }
        else if (direction == new Vector2(0, -1))
        {
            MoveUp();
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
        foreach (var museumTile in _museumRunningDataContainer.MuseumTiles)
        {
            if (museumTile.XPosition == tilePosition.X && museumTile.YPosition == tilePosition.Y)
            {
                if (museumTile.Walkable)
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
    private void IdleRight()
    {
        
        _characterPartsParent.Scale = new Vector2(1, 1);
        _animationPlayerInstance.Play("idle_front_facing");
        _playerFacingTheFront = true;
    }

    private void IdleLeft()
    {
        
        _characterPartsParent.Scale = new Vector2(1, 1);
        _animationPlayerInstance.Play("idle_back_facing");
        _playerFacingTheFront = false;
    }

    private void IdleDown()
    {
        
        _characterPartsParent.Scale = new Vector2(-1, 1);
        _animationPlayerInstance.Play("idle_front_facing");
        _playerFacingTheFront = true;
    }

    private void IdleUp()
    {
        
        _characterPartsParent.Scale = new Vector2(-1, 1);
        _animationPlayerInstance.Play("idle_back_facing");
        _playerFacingTheFront = false;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MuseumActions.OnTimePauseValueUpdated -= OnTimePauseValueUpdated;
        MuseumActions.OnClickGuestAi -= OnClickGuestAi;

    }
}
