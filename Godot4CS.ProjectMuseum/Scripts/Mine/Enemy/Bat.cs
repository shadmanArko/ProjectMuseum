using System;
using System.Collections.Generic;
using System.Linq;
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
     public List<AStarNode> AStarNodes { get; private set; }
     private AStarPathfinding _aStarPathfinding;

     [Export] private AnimationPlayer _animationPlayer;
     
     [Export] public float SearchRadius = 30f;
     [Export] private Vector2 _targetPos;
     [Export] private float _movementSpeed;

     #region Initializers

     public override void _EnterTree()
     {
         
     }

     public override void _Ready()
     {
         InitializeDiReference();
         AStarNodes = new List<AStarNode>();
         _path = new List<PathNode>();
         _aStarPathfinding = new AStarPathfinding(false);
         MineActions.OnPlayerLandedIntoTheMine += SetChild;
         SetPhysicsProcess(false);
         _moveToPlayer = false;
         _animationPlayer.Play("fly");
     }
     
     private void SetChild()
     {
         // _mineGenerationVariables.MineGenView.AddChild(this);
         GlobalPosition = new Vector2(480, 100) + new Vector2(20,20);
         // SetPhysicsProcess(true);
     }

     private void InitializeDiReference()
     {
         _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
         _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
     }

     #endregion

     public override void _PhysicsProcess(double delta)
     {
         SearchForPlayer();
         if(_moveToPlayer)
             MoveToPlayer();
     }

     private List<PathNode> _path;
     [Export] private bool _moveToPlayer; 
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
//TODO: USE LIST INSTEAD
                 _path.Clear();
                 foreach (var pathNode in tempPath)
                 {
                     var node = new PathNode
                     {
                         Position = pathNode * cellSize + cellOffset,
                         IsVisited = false
                     };
                         
                     _path.Add(node);
                 }

                 #endregion
             }
             else
             {
                 //cannot find path to player. do sth else
                 _moveToPlayer = false;
                 // SetPhysicsProcess(false);
                 return;
             }
                 
             // GD.Print($"path calculated: {_path.Count}");
             // _moveToPlayer = _path.Count > 0;
         }
         else
         {
             _moveToPlayer = true;
             GD.Print($"move to player set to {_moveToPlayer}");
         }
             
         GD.Print($"path count: {_path.Count}");
         GD.Print($"last node of path {_path[^1]}");
         GD.Print("Player found within radius at distance: " + distance);
     }

     private void MoveToPlayer()
     {
         for (var index = 0; index < _path.Count; index++)
         {
             var pathNode = _path[index];
             if (pathNode.Position.DistanceTo(GlobalPosition) <= 1)
                 pathNode.IsVisited = true;
         }

         for (var i = 0; i < _path.Count; i++)
             if (_path[i].IsVisited)
             {
                 _path[i].Dispose();
                 _path.Remove(_path[i]);
             }

         if (_path.Count <= 0)
         {
             _moveToPlayer = false;
             return;
         }
         
         var targetPos = _path[0].Position;
         var direction = (targetPos - Position).Normalized();
         Velocity = new Vector2(_movementSpeed, _movementSpeed) * direction;
         MoveAndSlide();
     }

     private List<Vector2I> FindPath()
     {
         foreach (var cell in _mineGenerationVariables.Mine.Cells)
         {
             bool isWalkable = cell.IsBroken;
             AStarNode aStarNode = new AStarNode(cell.PositionX, cell.PositionY, null, 0f, 0f, isWalkable);
             AStarNodes.Add(aStarNode);
         }

         GD.Print($"getCellPos BAT: {GetCellPos(Position)}");
         GD.Print($"getCellPos PLAYER: {GetCellPos(_playerControllerVariables.Position)}");
         GD.Print($"AStarNodes is null: {AStarNodes == null}");
         
         var path = _aStarPathfinding.FindPath(GetCellPos(Position)*-1, GetCellPos(_playerControllerVariables.Position)*-1,
             AStarNodes);
         if (path != null)
         {
             for (int i = 0; i < path.Count; i++)
             {
                 var temp = path[i];
                 path[i] = new Vector2I(Mathf.Abs(temp.X), Mathf.Abs(temp.Y));
             }
             
             for (var index = 0; index < path.Count; index++)
             {
                 GD.Print($"path {index} is ({path[index].X},{path[index].Y})");
             }
         }
         else
         {
             path = new List<Vector2I> { GetCellPos(Position)};
             GD.PrintErr("Fatal error: path is null");
         }

         return path;
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

public struct PathNode : IDisposable
{
    public Vector2 Position { get; set; }
    public bool IsVisited { get; set; }

    public void Dispose()
    {
        
    }
}