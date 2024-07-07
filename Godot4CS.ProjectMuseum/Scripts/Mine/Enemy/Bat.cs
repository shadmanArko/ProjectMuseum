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
     
     [Export]
     public float SearchRadius = 30f;

     #region Initializers

     public override void _EnterTree()
     {
         _path = new List<Vector2I>();
     }

     public override void _Ready()
     {
         InitializeDiReference();
         AStarNodes = new List<AStarNode>();
         _path = new List<Vector2I>();
         _aStarPathfinding = new AStarPathfinding(false);
         MineActions.OnPlayerLandedIntoTheMine += SetChild;
         SetProcess(false);
         _moveToPlayer = false;
         _animationPlayer.Play("fly");
     }
     
     private void SetChild()
     {
         _mineGenerationVariables.MineGenView.AddChild(this);
         GlobalPosition = new Vector2(480, 100);
         SetPhysicsProcess(false);
     }

     private void InitializeDiReference()
     {
         _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
         _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
     }

     #endregion

     public override void _PhysicsProcess(double delta)
     {
         var targetPos = new Vector2(600, 100);
         var direction = (targetPos - Position).Normalized();
         Velocity += direction;
         MoveAndSlide();
     }

     private List<Vector2I> _path;
     [Export] private bool _moveToPlayer; 
     private void SearchForPlayer()
     {
         // Calculate the distance between the bat and the player
         float distance = Position.DistanceTo(_playerControllerVariables.Position);
         GD.Print($"player to enemy distance: {distance}");
         if (distance <= SearchRadius)
         {
             if (_path.Count <= 0 || _path[^1] != GetCellPos(_playerControllerVariables.Position))
             {
                 _path = FindPath();
                 GD.Print($"path calculated: {_path.Count}");
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
     }

     private void MoveToPlayer()
     {
         var cellSize = _mineGenerationVariables.Mine.CellSize;
         Velocity.MoveToward(new Vector2(_path[0].X, _path[0].Y) * cellSize, 0.5f);
         // foreach (var node in _path)
         // {
         //     var pos = new Vector2(node.X, node.Y) * cellSize;
         //     if (Position.DistanceTo(pos) > 0)
         //     {
         //         Velocity.MoveToward(pos, 0.5f);
         //         MoveAndSlide();
         //     }
         // }
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
             SearchForPlayer();
             _moveToPlayer = true;
             GD.Print("Searching for player");
         }
     }

     #region Utilities

     private Vector2I GetCellPos(Vector2 globalPos)
     {
         return _mineGenerationVariables.MineGenView.LocalToMap(globalPos);
     }

     #endregion
}