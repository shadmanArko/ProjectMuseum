using System;
using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;
using Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;
using ProjectMuseum.Models;

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
		MineActions.OnPlayerDigActionPressed += AttackWall;
		MineActions.OnPlayerBrushActionPressed += BrushWall;
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
		}
	}
	
	#region Wall Attack Detection
    
	private void AttackWall()
	{
		var targetTilePosition = FindPositionOfTargetCell();
		if (!IsCellBreakValid(targetTilePosition)) return;
        
		var cell = _mineGenerationVariables.Cells[targetTilePosition.X, targetTilePosition.Y];
		if (cell.HasArtifact)
			DigArtifactCell(cell, targetTilePosition);
		else
			DigOrdinaryCell(cell, targetTilePosition);
	}

	private void BrushWall()
	{
		var targetTilePosition = FindPositionOfTargetCell();
		if (!IsCellBreakValid(targetTilePosition)) return;
		var cell = _mineGenerationVariables.Cells[targetTilePosition.X, targetTilePosition.Y];
		BrushOutArtifact(cell, targetTilePosition);
	}

	[Export] private string _alternateButtonPressMiniGameScenePath;
	private void BrushOutArtifact(Cell cell, Vector2I tilePos)
	{
		if (!cell.HasArtifact || cell.HitPoint != 1) return;
		_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.GoldArtifactSourceId,new Vector2I(3,0));
		//TODO: Pop up that says "Extracting Artifact"
		_playerControllerVariables.CanMove = false;
		var scene = ResourceLoader.Load<PackedScene>(_alternateButtonPressMiniGameScenePath).Instantiate() as AlternateTapMiniGame;
		if (scene is null)
		{
			GD.PrintErr("COULD NOT instantiate Alternate tap mini game scene. FATAL ERROR");
			return;
		}
		AddChild(scene);
		//TODO: Start a mini game
		GD.Print("Extracted Artifact");
		RevealAdjacentWalls(tilePos);
	}

	private Vector2I FindPositionOfTargetCell()
	{
		var tilePos = _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
		tilePos += _playerControllerVariables.MouseDirection;
		GD.Print($"Breaking Cell{tilePos}");

		return tilePos;
	}

	private bool IsCellBreakValid(Vector2I tilePos)
	{
		if (tilePos.X < 0 || tilePos.Y < 0)
		{
			GD.Print("Wrong cell index");
			return false;
		}
		var cell = _mineGenerationVariables.Cells[tilePos.X, tilePos.Y];
		if (!cell.IsBreakable)
		{
			GD.Print("Is not breakable");
			return false;
		}

		return true;
	}

	private void DigArtifactCell(Cell cell, Vector2I tilePos)
	{
		_mineGenerationVariables.Cells[tilePos.X, tilePos.Y].HitPoint--;
		Math.Clamp(-_mineGenerationVariables.Cells[tilePos.X, tilePos.Y].HitPoint, 0, 100);
        
		if (cell.HitPoint >= 2)
			_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.GoldArtifactSourceId,new Vector2I(1,0));
		else if (cell.HitPoint >= 1)
			_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.GoldArtifactSourceId,new Vector2I(2,0));
		else
		{
			_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(4,0));
			//TODO: Pop up that says "Artifact Destroyed"
			GD.Print("Artifact destroyed");
			RevealAdjacentWalls(tilePos);
		}
	}

	private void DigOrdinaryCell(Cell cell, Vector2I tilePos)
	{
		_mineGenerationVariables.Cells[tilePos.X, tilePos.Y].HitPoint--;
		Math.Clamp(-_mineGenerationVariables.Cells[tilePos.X, tilePos.Y].HitPoint, 0, 100);
		
		if (cell.HitPoint >= 2)
			_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(1,0));
		else if (cell.HitPoint >= 1)
			_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(2,0));
		else
		{
			_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(4,0));
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
			if (cell.IsRevealed || !cell.IsInstantiated || !cell.IsBreakable || cell.HitPoint <= 0) continue;
			
			cell.IsRevealed = true;
			_mineGenerationVariables.MineGenView.SetCell(0,tilePosition,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(0,0));
		}
	}
    
	#endregion


}