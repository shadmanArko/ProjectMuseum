using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Plugins.AStarPathFinding;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Bat : CharacterBody2D
{
     private PlayerControllerVariables _playerControllerVariables;
     private MineGenerationVariables _mineGenerationVariables;
     private FlyingEnemyAi _enemyAi;
     private RandomNumberGenerator _rand;
     private AStarPathfinding _aStarPathfinding;
     private List<Vector2> _path;

     [Export] private AnimationPlayer _animationPlayer;

     [Export] private FlyingEnemyPhase _phase;

     public FlyingEnemyPhase Phase
     {
         get => _phase;
         set
         {
             _phase = value;
             GD.Print($"phase changed to {_phase}");
         }
     }

     [Export] private float _searchRadius = 30f;
     [Export] private float _attackRadius = 20f;

     [Export] private float _restTime = 3f;
     [Export] private float _exhaustionTime = 10f;

     [Export] private float _timer;
     
     [Export] private float _movementSpeed;
     
     [Export] private Vector2 _targetPos;
     
     [Export] private bool _moveAlongPath; 
     [Export] private bool _attackPlayer; 

     #region Initializers
     

     public override void _Ready()
     {
         InitializeDiReference();
         _enemyAi = new FlyingEnemyAi();
         _path = new List<Vector2>();
         _rand = new RandomNumberGenerator();
         _aStarPathfinding = new AStarPathfinding(false);
         MineActions.OnPlayerLandedIntoTheMine += SetChild;
         SetPhysicsProcess(false);
         _moveAlongPath = false;
         _animationPlayer.Play("fly");
     }
     
     private void InitializeDiReference()
     {
         _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
         _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
     }
     
     private void SetChild()
     {
         GlobalPosition = new Vector2(480, 100) + new Vector2(20,20);
     }
     
     #endregion

     public override async void _PhysicsProcess(double delta)
     {
         // SearchForPlayer();
         
         if (_phase == FlyingEnemyPhase.Chase)
         {
             if (_moveAlongPath)
             {
                 MoveAlongPath();
             }
             else
             {
                 ChasePlayer();
             }
         }
         else if (_phase == FlyingEnemyPhase.Rest)
         {
             if (_moveAlongPath)
             {
                 if (_path.Count > 0)
                 {
                     MoveAlongPath();
                 }
                 else
                 {
                     #region Rest

                     if (_restTime > 0)
                     {
                         _restTime -= (float) delta;
                         _animationPlayer.Play("hang");
                         await Task.Delay(Mathf.CeilToInt(_animationPlayer.CurrentAnimationLength * 1000));
                     }
                     else
                     {
                         _animationPlayer.Play("hang_to_fly");
                         await Task.Delay(Mathf.CeilToInt(_animationPlayer.CurrentAnimationLength * 1000));
                         _phase = FlyingEnemyPhase.Chase;
                         _animationPlayer.Play("fly");
                     }

                     #endregion
                 }
             }
             else
             {
                 FindRestingPlace();
             }
         }
         else if (_phase == FlyingEnemyPhase.Explore)
         {
             if (_timer > 0)
             {
                 _timer -= (float)delta;
             }
             else
             {
                 _phase = FlyingEnemyPhase.Rest;
                 _path.Clear();
                 _restTime = 5f;
             }
         }
         
         
     }


     #region Chase

     private void SearchForPlayer()
     {
         var distance = Position.DistanceTo(_playerControllerVariables.Position);
         
         if (!(distance <= _searchRadius))
         {
             // _moveToPlayer = false;
             //TODO: do sth else
             // _phase = FlyingEnemyPhase.Explore;
             // _timer = _exhaustionTime;
         }
         else
         {
             _phase = FlyingEnemyPhase.Chase;
         }
     }

     private void ChasePlayer()
     {
         if (_path.Count <= 0) //if no path found or path not updated
         {
             var tempPath = FindPath(GetCellPos(_playerControllerVariables.Position));
             if (tempPath.Count > 0)
             {
                 #region path to player not found. doing sth else

                 if (tempPath[^1] != GetCellPos(_playerControllerVariables.Position))
                 {
                     _moveAlongPath = false; 
                     //TODO: do sth else. path to player not found
                     _phase = FlyingEnemyPhase.Explore;
                     _timer = _exhaustionTime;
                     // SetPhysicsProcess(false);
                     return;
                 }

                 #endregion

                 #region Creating new path for bat to move to player

                 SetPath(tempPath);

                 #endregion
             }
             else
             {
                 // _moveAlongPath = false;
                 //TODO: do sth else
                 _moveAlongPath = false;
                 _phase = FlyingEnemyPhase.Explore;
                 _timer = _exhaustionTime;
             }
         }
         else
         {
             _moveAlongPath = true;
             //TODO: move along path
             _phase = FlyingEnemyPhase.Chase;
         }

     }
    

     private void MoveAlongPath()
     {
         for (var index = 0; index < _path.Count; index++)
         {
             var pathNode = _path[index];
             if (pathNode.DistanceTo(GlobalPosition) <= 1 || pathNode.X <= 0 || pathNode.Y <= 0)
                 _path.Remove(pathNode);
         }
         
         if (_path.Count <= 0)
         {
             _moveAlongPath = false;
             return;
         }
         
         var targetPos = _path[0];
         var direction = (targetPos - Position).Normalized();
         Velocity = new Vector2(_movementSpeed, _movementSpeed) * direction;
         MoveAndSlide();
     }

     #endregion

     #region Rest

     // private bool CheckIfExhausted()
     // {
     //     
     // }
     
     private void FindRestingPlace()
     {
         var listOfRestingPlaces = _enemyAi.FindRestingTiles(GetCellPos(Position), _mineGenerationVariables);
         if (listOfRestingPlaces.Count <= 0)
         {
             _timer = _exhaustionTime;
             _phase = FlyingEnemyPhase.Explore;
             return;
         }
         // foreach (var place in listOfRestingPlaces)
         // {
         //     var tempPath = FindPath(place);
         //     if(tempPath == null) continue;
         //     SetPath(tempPath);
         //     _moveAlongPath = true;
         //     _timer = _restTime;
         //     return;
         // }

         for (int i = 0; i < listOfRestingPlaces.Count; i++)
         {
             var randomNumber = _rand.RandiRange(0, listOfRestingPlaces.Count);
             var randomPos = listOfRestingPlaces[randomNumber];
             var tempPath = FindPath(randomPos);
             if (tempPath == null)
             {
                 listOfRestingPlaces.Remove(randomPos);
                 continue;
             }
             
             SetPath(tempPath);
             _moveAlongPath = true;
             _timer = _restTime;
             return;
         }

         _phase = FlyingEnemyPhase.Explore;
         _timer = _exhaustionTime;
     }

     [Export] private bool _isResting;
     private void Rest()
     {
         
             
     }

     #endregion

     #region Explore

     private void Explore()
     {
         
     }

     #endregion

     #region Attack

     private async Task AttackPlayer()
     {
         
     }

     #endregion
     
     #region To Be Removed In The Future

     public override void _Input(InputEvent @event)
     {
         if (@event.IsActionPressed("Enemy"))
         {
             SetPhysicsProcess(true);
             if (_path == null)
             {
                 GD.Print("Path is null");
                 // SetPhysicsProcess(false);
                 // return;
             }
             
             // if (_path.Count > 0)
             // {
             //     _moveAlongPath = true;
             // }
             // else
             // {
             //     GD.Print("Path count is 0");
             // }
         }
     }

     #endregion

     #region Utilities
     
     private void SetPath(List<Vector2I> tempPath)
     {
         var cellSize = _mineGenerationVariables.Mine.CellSize;
         var cellOffset = new Vector2(cellSize, cellSize) / 2;
         
         _path.Clear();
         foreach (var pathNode in tempPath)
         {
             var randomX = _rand.RandfRange(5, 10);
             var randomY = _rand.RandfRange(5, 15);
             var pos = pathNode * cellSize + cellOffset + new Vector2(randomX, randomY);
             _path.Add(pos);
         }
     }

     private List<Vector2I> FindPath(Vector2I targetPos)
     {
         var path = _aStarPathfinding.FindPath(GetCellPos(Position)*-1, targetPos * -1,
             _mineGenerationVariables.PathfindingNodes);
         if (path != null)
         {
             for (int i = 0; i < path.Count; i++)
             {
                 var temp = path[i];
                 path[i] = new Vector2I(Mathf.Abs(temp.X), Mathf.Abs(temp.Y));
             }
         }
         else
         {
             path = new List<Vector2I> { GetCellPos(Position)};
         }

         return path;
     }
     
     private Vector2I GetCellPos(Vector2 globalPos)
     {
         return _mineGenerationVariables.MineGenView.LocalToMap(globalPos);
     }

     #endregion

     #region Finalizers

     public override void _ExitTree()
     {
         SetPhysicsProcess(false);
     }

     #endregion
}