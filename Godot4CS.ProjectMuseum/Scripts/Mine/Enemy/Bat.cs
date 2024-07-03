using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Plugins.AStarPathFinding;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Bat : Node2D
{
     private PlayerControllerVariables _playerControllerVariables;
     private MineGenerationVariables _mineGenerationVariables;
     public List<AStarNode> AStarNodes { get; private set; }
     private AStarPathfinding _aStarPathfinding;
     
     [Export]
     public float SearchRadius = 100f;

     // Name of the player node
     [Export]
     public string PlayerName = "Player";

     // Reference to the player node
     private Node2D player;

     public override void _Ready()
     {
         InitializeDiReference();
         AStarNodes = new List<AStarNode>();
         _aStarPathfinding = new AStarPathfinding(false);
         MineActions.OnPlayerLandedIntoTheMine += SetChild;
         player = GetNode<Node2D>("/root/" + PlayerName);
     }

     public override void _Process(double delta)
     {
         base._Process(delta);
         SearchForPlayer();
     }

     private void SearchForPlayer()
     {
         if (player == null)
             return;

         // Calculate the distance between the bat and the player
         float distance = Position.DistanceTo(player.Position);
         if (distance <= SearchRadius)
         {
             GD.Print("Player found within radius at distance: " + distance);
             // Add your logic for what happens when the player is found
         }
     }

     private void SetChild()
     {
         _mineGenerationVariables.MineGenView.AddChild(this);
         GlobalPosition = new Vector2(480, 100);
     }

     private void InitializeDiReference()
     {
         _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
         _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
     }

     private void FindPath()
     {
         foreach (var cell in _mineGenerationVariables.Mine.Cells)
         {
             bool isWalkable = cell.IsBroken;
             AStarNode aStarNode = new AStarNode(cell.PositionX, cell.PositionY, null, 0f, 0f, isWalkable);
             AStarNodes.Add(aStarNode);
         }

         GD.Print($"getCellPos BAT: {GetCellPos(Position)}");
         GD.Print($"getCellPos PLAYER: {GetCellPos(_playerControllerVariables.Position)}");
         GD.Print($"AstarNodes is null: {AStarNodes == null}");
         
         var path = _aStarPathfinding.FindPath(GetCellPos(Position) * -1, GetCellPos(_playerControllerVariables.Position) * -1,
             AStarNodes);
         if (path != null)
         {
             for (var index = 0; index < path.Count; index++)
             {
                 GD.Print($"path {index} is ({path[index].X},{path[index].Y})");
             }
         }
     }

     public override void _Input(InputEvent @event)
     {
         if (@event.IsActionPressed("Enemy"))
         {
             FindPath();
         }
     }

     #region Utilities

     private Vector2I GetCellPos(Vector2 globalPos)
     {
         return _mineGenerationVariables.MineGenView.LocalToMap(globalPos);
     }

     #endregion
}