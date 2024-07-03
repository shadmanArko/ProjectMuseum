using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Plugins.AStarPathFinding;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Bat:Node
{
    private MineGenerationVariables _mineGenerationVariables;
    public List<AStarNode> AStarNodes { get; private set; }
     private AStarPathfinding _aStarPathfinding;

     public override void _Ready()
     {
         _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
         base._Ready();
         _aStarPathfinding = new AStarPathfinding(false);
         
         FindPath();
     }

     private void FindPath()
     {
         foreach (var cell in _mineGenerationVariables.Mine.Cells)
         {
             bool isWalkable = cell.IsBroken;
             AStarNode aStarNode = new AStarNode(cell.PositionX, cell.PositionY, null, 0f, 0f, isWalkable);
             AStarNodes.Add(aStarNode);
         }

         var path = _aStarPathfinding.FindPath(new Vector2I(0, 0), new Vector2I(1, 1), AStarNodes);
     }
}