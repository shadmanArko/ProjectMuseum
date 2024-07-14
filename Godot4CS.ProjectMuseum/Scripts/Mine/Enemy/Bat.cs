using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Plugins.AStarPathFinding;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Bat : CharacterBody2D
{
     private PlayerControllerVariables _playerControllerVariables;
     private MineGenerationVariables _mineGenerationVariables;
     private RandomNumberGenerator _rand;
     private AStarPathfinding _aStarPathfinding;
     private List<Vector2> _path;

     [Export] private AnimationPlayer _animationPlayer;
     [Export] public float SearchRadius = 30f;
     [Export] private Vector2 _targetPos;
     [Export] private float _movementSpeed;
     [Export] private bool _moveToPlayer; 

     #region Initializers
     

     public override void _Ready()
     {
         InitializeDiReference();
         _path = new List<Vector2>();
         _rand = new RandomNumberGenerator();
         _aStarPathfinding = new AStarPathfinding(false);
         MineActions.OnPlayerLandedIntoTheMine += SetChild;
         SetPhysicsProcess(false);
         _moveToPlayer = false;
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

     public override void _PhysicsProcess(double delta)
     {
         SearchForPlayer();
         if(_moveToPlayer)
             MoveToPlayer();
     }

    
     private void SearchForPlayer()
     {
         var cellSize = _mineGenerationVariables.Mine.CellSize;
         var cellOffset = new Vector2(cellSize, cellSize) / 2;
         var distance = Position.DistanceTo(_playerControllerVariables.Position);
         
         if (!(distance <= SearchRadius))
         {
             _moveToPlayer = false;
             return;
         }
         
         if (_path.Count <= 0) //if no path found or path not updated
         {
             GD.Print("path is null or player position not updated");
             var tempPath = FindPath();
             if (tempPath.Count > 0)    //populate _path with the new path found
             {
                 #region path to player not found. doing sth else

                 if (tempPath[^1] != GetCellPos(_playerControllerVariables.Position))
                 {
                     _moveToPlayer = false; //do sth else. path to player not found
                     SetPhysicsProcess(false);
                     GD.Print("path to player not found. doing sth else.");
                     return;
                 }

                 #endregion

                 #region Creating new path for bat to move to player
                 _path.Clear();
                 foreach (var pathNode in tempPath)
                 {
                     var randomX = _rand.RandfRange(5, 10);
                     var randomY = _rand.RandfRange(5, 15);
                     var pos = pathNode * cellSize + cellOffset + new Vector2(randomX, randomY);
                     _path.Add(pos);
                 }

                 #endregion
             }
             else
             {
                 _moveToPlayer = false;
                 return;
             }
             
         }
         else
         {
             _moveToPlayer = true;
         }
     }
     
     private List<Vector2I> FindPath()
     {
         var path = _aStarPathfinding.FindPath(GetCellPos(Position)*-1, GetCellPos(_playerControllerVariables.Position)*-1,
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

     private void MoveToPlayer()
     {
         for (var index = 0; index < _path.Count; index++)
         {
             var pathNode = _path[index];
             if (pathNode.DistanceTo(GlobalPosition) <= 1 || pathNode.X <= 0 || pathNode.Y <= 0)
                 _path.Remove(pathNode);
         }
         

         if (_path.Count <= 0)
         {
             _moveToPlayer = false;
             return;
         }
         
         var targetPos = _path[0];
         var direction = (targetPos - Position).Normalized();
         Velocity = new Vector2(_movementSpeed, _movementSpeed) * direction;
         MoveAndSlide();
     }

     

     public override void _Input(InputEvent @event)
     {
         if (@event.IsActionPressed("Enemy"))
         {
             // SearchForPlayer();
             SetPhysicsProcess(true);
             if (_path == null)
             {
                 GD.Print("Path is null");
                 SetPhysicsProcess(false);
                 return;
             }
             
             if (_path.Count > 0)
             {
                 _moveToPlayer = true;
                 // SetPhysicsProcess(true);
             }
             else
             {
                 GD.Print("Path count is 0");
                 // SetPhysicsProcess(false);
             }
             
             
             GD.Print("Searching for player");
         }
     }

     #region Utilities

     private Vector2I GetCellPos(Vector2 globalPos)
     {
         return _mineGenerationVariables.MineGenView.LocalToMap(globalPos);
     }

     #endregion

     public override void _ExitTree()
     {
         SetPhysicsProcess(false);
     }
}