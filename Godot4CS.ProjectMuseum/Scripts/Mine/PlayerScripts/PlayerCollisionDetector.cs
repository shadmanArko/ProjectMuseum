using System;
using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerCollisionDetector : Node2D
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;
	public override void _Ready()
	{
		InitializeDiReferences();
		SubscribeToActions();
	}

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerCollisionDetection += DetectCollision;
		MineActions.OnPlayerAttackAction += AttackWall;
	}
    
	private void DetectCollision(KinematicCollision2D collision)
	{
		var tileMap = _mineGenerationVariables.MineGenView.TileMap;
		if (collision.GetCollider() == tileMap)
		{
			var tilePos = _mineGenerationVariables.MineGenView.TileMap.LocalToMap(_playerControllerVariables.Position);
			var playerPos = _mineGenerationVariables.MineGenView.TileMap.LocalToMap(_playerControllerVariables.Position);
			tilePos -= (Vector2I) collision.GetNormal();

			if (tilePos.Y > playerPos.Y)
				_playerControllerVariables.IsGrounded = true;
            
			//GD.Print($"tilePos: {tilePos.X}, {tilePos.Y} | PlayerPos: {playerPos.X}, {playerPos.Y}");
		}
	}
	
		#region Wall Attack Detection
    
	private void AttackWall()
	{
		var tilePos = _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
		var newPos = _playerControllerVariables.MouseDirection;

		tilePos += newPos;
		GD.Print($"Breaking Cell{tilePos}");
		GD.Print($"mouse direction: {_playerControllerVariables.MouseDirection}");
		BreakCell(tilePos);
	}
	
	private void BreakCell(Vector2I tilePos)
	{
		if (tilePos.X < 0 || tilePos.Y < 0)
		{
			GD.Print("Wrong cell index");
			return;
		}
		var cell = _mineGenerationVariables.Cells[tilePos.X, tilePos.Y];
		if (!cell.IsBreakable)
		{
			GD.Print("Is not breakable");
			return;
		}
		
		_mineGenerationVariables.Cells[tilePos.X, tilePos.Y].BreakStrength--;
		Math.Clamp(-_mineGenerationVariables.Cells[tilePos.X, tilePos.Y].BreakStrength, 0, 100);
		
		if (cell.BreakStrength >= 2)
			_mineGenerationVariables.MineGenView.SetCell(0,tilePos,1,new Vector2I(1,0));
		else if (cell.BreakStrength >= 1)
			_mineGenerationVariables.MineGenView.SetCell(0,tilePos,1,new Vector2I(2,0));
		else
		{
			_mineGenerationVariables.MineGenView.SetCell(0,tilePos,1,new Vector2I(4,0));
			RevealAdjacentWalls(tilePos);
		}
	}

	private void RevealAdjacentWalls(Vector2I tilePos)
	{
		var tilePositions = new List<Vector2I>
		{
			tilePos + Vector2I.Up,
			tilePos + Vector2I.Down,
			tilePos + Vector2I.Left,
			tilePos + Vector2I.Right
		};

		foreach (var tilePosition in tilePositions)
		{
			var cell = _mineGenerationVariables.Cells[tilePosition.X, tilePosition.Y];

			if (cell is null) continue;
			if (cell.IsRevealed || !cell.IsInstantiated || !cell.IsBreakable || cell.BreakStrength <= 0) continue;
			
			cell.IsRevealed = true;
			_mineGenerationVariables.MineGenView.SetCell(0,tilePosition,1,new Vector2I(0,0));
		}
	}
    
	#endregion


}