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

		MineActions.OnMiniGameWon += MiniGameWon;
		MineActions.OnMiniGameLost += MiniGameLost;
		MineActions.OnArtifactDiscoveryOkayButtonPressed += TurnOffArtifactDiscoveryScene;
	}
    
	private void DetectCollision(KinematicCollision2D collision)
	{
		var tileMap = _mineGenerationVariables.MineGenView.TileMap;

		if (collision == null)
		{
			if (_playerControllerVariables.State != MotionState.Hanging)
			{
				_playerControllerVariables.State = MotionState.Falling;
			}
			
			return;
		}
		
		if (collision.GetCollider() == tileMap)
		{
			var tilePos = _mineGenerationVariables.MineGenView.TileMap.LocalToMap(_playerControllerVariables.Position);
			tilePos +=  Vector2I.Down;
			var cell = _mineGenerationVariables.GetCell(tilePos);

			if (cell.IsBroken)
			{
				if (_playerControllerVariables.State != MotionState.Hanging)
					_playerControllerVariables.State = MotionState.Falling;
			}
			else if (_playerControllerVariables.State != MotionState.Hanging)
				_playerControllerVariables.State = MotionState.Grounded;
		}
	}
	
	#region Wall Attack Detection
    
	private void AttackWall()
	{
		var targetTilePosition = FindPositionOfTargetCell();
		if (!IsCellBreakValid(targetTilePosition)) return;

		var cell = _mineGenerationVariables.GetCell(targetTilePosition);
		GD.Print($"breaking cell: {targetTilePosition}");
		if (cell.HasArtifact)
			DigArtifactCell(targetTilePosition);
		else
			DigOrdinaryCell(targetTilePosition);
	}

	private void BrushWall()
	{
		var targetTilePosition = FindPositionOfTargetCell();
		if (!IsCellBreakValid(targetTilePosition)) return;
		var cell = _mineGenerationVariables.GetCell(targetTilePosition);
		BrushOutArtifact(cell, targetTilePosition);
	}

	[Export] private string _alternateButtonPressMiniGameScenePath;
	[Export] private bool _isMiniGameLoaded;
	private Vector2I _artifactTilePos;
	private void BrushOutArtifact(Cell cell, Vector2I tilePos)
	{
		if (!cell.HasArtifact || cell.HitPoint != 1) return;
		//_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.GoldArtifactSourceId,new Vector2I(3,0));
		MineSetCellConditions.SetArtifactCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection, cell, _mineGenerationVariables.MineGenView);
		//TODO: Pop up that says "Extracting Artifact"
		_playerControllerVariables.CanMove = false;
		_artifactTilePos = tilePos;
		GD.Print("Loading mini game scene");
		var scene = ResourceLoader.Load<PackedScene>(_alternateButtonPressMiniGameScenePath).Instantiate() as AlternateTapMiniGame;
		if (scene is null)
		{                                                                                                                    
			GD.PrintErr("COULD NOT instantiate Alternate tap mini game scene. FATAL ERROR");
			return;
		}
		AddChild(scene);
	}

	private void MiniGameWon()
	{
		GD.Print("Successfully Extracted Artifact");

		ShowDiscoveredArtifact();
		DigOrdinaryCell(_artifactTilePos);
		//RevealAdjacentWalls(_artifactTilePos);
	}
	
	private void MiniGameLost()
	{
		GD.Print("Failed to Extract Artifact");
		DigOrdinaryCell(_artifactTilePos);
		//RevealAdjacentWalls(_artifactTilePos);
		_playerControllerVariables.CanMove = true;
	}

	[Export] private string _discoveredArtifactScenePath;
	private void ShowDiscoveredArtifact()
	{
		var scene = ResourceLoader.Load<PackedScene>(_discoveredArtifactScenePath).Instantiate() as DiscoveredArtifactVisualizer;
		if (scene is null)
		{                                                                                                                    
			GD.PrintErr("COULD NOT instantiate Alternate tap mini game scene. FATAL ERROR");
			return;
		}
		AddChild(scene);
	}

	private void TurnOffArtifactDiscoveryScene()
	{
		_playerControllerVariables.CanMove = true;
	}

	

	private Vector2I FindPositionOfTargetCell()
	{
		var tilePos = _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
		tilePos += _playerControllerVariables.MouseDirection;
		// GD.Print($"Mouse position: {_playerControllerVariables.MouseDirection}");
		// GD.Print($"Breaking Cell{tilePos}");

		return tilePos;
	}

	private bool IsCellBreakValid(Vector2I tilePos)
	{
		if (tilePos.X < 0 || tilePos.Y < 0)
		{
			GD.Print("Wrong cell index");
			return false;
		}

		var cell = _mineGenerationVariables.GetCell(tilePos);
		if (!cell.IsBreakable)
		{
			GD.Print("Is not breakable");
			return false;
		}

		return true;
	}

	private void DigArtifactCell(Vector2I tilePos)
	{
		GD.Print("Inside artifact break method");
		var cell = _mineGenerationVariables.GetCell(tilePos);
		cell.HitPoint--;
		Math.Clamp(-_mineGenerationVariables.GetCell(tilePos).HitPoint, 0, 10000);
        
		MineSetCellConditions.SetArtifactCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection,cell ,_mineGenerationVariables.MineGenView);
		
		if (cell.HitPoint <= 0)
		{
			var cells = MineCellDestroyer.DestroyCellByPosition(tilePos, _mineGenerationVariables);

			GD.Print($"mouseDirection: {_playerControllerVariables.MouseDirection}");
			if (_playerControllerVariables.MouseDirection == Vector2I.Down)
			{
				GD.Print("is grounded is made false");
				if(_playerControllerVariables.State != MotionState.Hanging)
					_playerControllerVariables.State = MotionState.Falling;
			}
			
			foreach (var tempCell in cells)
			{
				var tempCellPos = new Vector2I(tempCell.PositionX, tempCell.PositionY);
				MineSetCellConditions.SetTileMapCell(tempCellPos, _playerControllerVariables.MouseDirection, tempCell, _mineGenerationVariables.MineGenView);
			}
		}
		
		// if (cell.HitPoint >= 2)
		// 	_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.GoldArtifactSourceId,new Vector2I(1,0));
		// else if (cell.HitPoint >= 1)
		// 	_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.GoldArtifactSourceId,new Vector2I(2,0));
		// else
		// {
		// 	_mineGenerationVariables.MineGenView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(5,2));
		// 	//TODO: Pop up that says "Artifact Destroyed"
		// 	GD.Print("Artifact destroyed");
		// 	RevealAdjacentWalls(tilePos);
		// }
	}

	private void DigOrdinaryCell(Vector2I tilePos)
	{
		var cell = _mineGenerationVariables.GetCell(tilePos);
		cell.HitPoint--;
		Math.Clamp(-_mineGenerationVariables.GetCell(tilePos).HitPoint, 0, 10000);

		MineSetCellConditions.SetCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection,cell ,_mineGenerationVariables.MineGenView);
		if (cell.HitPoint <= 0)
		{
			var cells = MineCellDestroyer.DestroyCellByPosition(tilePos, _mineGenerationVariables);

			GD.Print($"mouseDirection: {_playerControllerVariables.MouseDirection}");
			if (_playerControllerVariables.MouseDirection == Vector2I.Down)
			{
				GD.Print("is grounded is made false");
				if(_playerControllerVariables.State != MotionState.Hanging)
					_playerControllerVariables.State = MotionState.Falling;
			}
			
			foreach (var tempCell in cells)
			{
				var tempCellPos = new Vector2I(tempCell.PositionX, tempCell.PositionY);
				MineSetCellConditions.SetTileMapCell(tempCellPos, _playerControllerVariables.MouseDirection, tempCell, _mineGenerationVariables.MineGenView);
			}
		}
	}

	private void SetCracksOnTiles(Vector2I tilePos, Vector2I coords)
	{
		var mouseDir = _playerControllerVariables.MouseDirection;
		switch (mouseDir)
		{
			case Vector2I(1,0):
				_mineGenerationVariables.MineGenView.SetCell(1,tilePos,_mineGenerationVariables.MineGenView.TileCrackSourceId,coords,1);
				break;
			case Vector2I(-1,0):
				_mineGenerationVariables.MineGenView.SetCell(1,tilePos,_mineGenerationVariables.MineGenView.TileCrackSourceId,coords);
				break;
			case Vector2I(0,-1):
				_mineGenerationVariables.MineGenView.SetCell(1,tilePos,_mineGenerationVariables.MineGenView.TileCrackSourceId,coords,2);
				break;
			case Vector2I(0,1):
				_mineGenerationVariables.MineGenView.SetCell(1,tilePos,_mineGenerationVariables.MineGenView.TileCrackSourceId,coords,3);
				break;
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
			var cell = _mineGenerationVariables.GetCell(tilePos);

			if (cell is null) continue;
			if (cell.IsRevealed || !cell.IsInstantiated || !cell.IsBreakable || cell.HitPoint <= 0) continue;
			
			cell.IsRevealed = true;
			_mineGenerationVariables.MineGenView.SetCell(0,tilePosition,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(1,1));
		}
	}
    
	#endregion


}