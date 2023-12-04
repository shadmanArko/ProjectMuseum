using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;
using Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerCollisionDetector : Node2D
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;
	private MineCellCrackMaterial _mineCellCrackMaterial;
	private List<RawArtifactDescriptive> _rawArtifactDescriptive;
	private List<RawArtifactFunctional> _rawArtifactFunctional;

	private HttpRequest _getMineArtifactHttpRequest;

	public override void _Ready()
	{
		CreateHttpRequests();
		InitializeDiReferences();
		SubscribeToActions();
	}
    

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
		_mineCellCrackMaterial = ServiceRegistry.Resolve<MineCellCrackMaterial>();
		_rawArtifactDescriptive = ServiceRegistry.Resolve<List<RawArtifactDescriptive>>();
		_rawArtifactFunctional = ServiceRegistry.Resolve<List<RawArtifactFunctional>>();
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

	#region Http Requests

	private void CreateHttpRequests()
	{
		_getMineArtifactHttpRequest = new HttpRequest();
		AddChild(_getMineArtifactHttpRequest);
		_getMineArtifactHttpRequest.RequestCompleted += OnGetMineArtifactHttpRequestCompleted;
	}

	private void OnGetMineArtifactHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var artifact = JsonSerializer.Deserialize<Artifact>(jsonStr);
		var rawArtifact = _rawArtifactFunctional.FirstOrDefault(raw => raw.Id == artifact.RawArtifactId);

		var rand = new RandomNumberGenerator();
		var randomMat = rawArtifact!.Materials[rand.RandiRange(0, rawArtifact!.Materials.Count)];
		var cellCrackMaterial =
			_mineCellCrackMaterial.CellCrackMaterials.FirstOrDefault(mat => mat.MaterialType == randomMat);
		
	}

	#endregion
    
	private void DetectCollision(KinematicCollision2D collision)
	{
		var tileMap = _mineGenerationVariables.MineGenView.TileMap;
        
        if (collision.GetCollider() == tileMap)
        {
	        var state = _playerControllerVariables.State;
			var tilePos = _mineGenerationVariables.MineGenView.TileMap.LocalToMap(_playerControllerVariables.Position);
			tilePos +=  Vector2I.Down;
			var cell = _mineGenerationVariables.GetCell(tilePos);

			if (cell.IsBroken) return;
			if (state != MotionState.Hanging)
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
		
		MineActions.OnSuccessfulDigActionCompleted?.Invoke();
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
		
		var cellCrackMaterial =
			_mineCellCrackMaterial.CellCrackMaterials.FirstOrDefault(tempCell =>tempCell.MaterialType == cell.ArtifactMaterial);
		
		MineSetCellConditions.SetArtifactCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection, cell, cellCrackMaterial, _mineGenerationVariables.MineGenView);
		
		_playerControllerVariables.CanMove = false;
		_artifactTilePos = tilePos;
		
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
	}
	
	private void MiniGameLost()
	{
		GD.Print("Failed to Extract Artifact");
		DigOrdinaryCell(_artifactTilePos);
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

		if (cell.IsBroken)
		{
			GD.Print("cell is already broken");
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

		var cellCrackMaterial =
			_mineCellCrackMaterial.CellCrackMaterials.FirstOrDefault(tempCell =>
				tempCell.MaterialType == cell.ArtifactMaterial);
		
		MineSetCellConditions.SetArtifactCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection,cell, cellCrackMaterial ,_mineGenerationVariables.MineGenView);
		
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
				MineSetCellConditions.SetTileMapCell(tempCellPos, _playerControllerVariables.MouseDirection, tempCell, cellCrackMaterial, _mineGenerationVariables.MineGenView);
			}
		}
	}

	private void DigOrdinaryCell(Vector2I tilePos)
	{
		var cell = _mineGenerationVariables.GetCell(tilePos);
		cell.HitPoint--;
		Math.Clamp(-_mineGenerationVariables.GetCell(tilePos).HitPoint, 0, 10000);
        
		 var normalCellCrackMaterial =
		 	_mineCellCrackMaterial!.CellCrackMaterials.FirstOrDefault(cellCrackMat =>
				cellCrackMat.MaterialType == "Normal");
        GD.Print("normal crack mat is null: "+(normalCellCrackMaterial == null));
		GD.Print(normalCellCrackMaterial.MaterialType);
		MineSetCellConditions.SetCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection,cell, normalCellCrackMaterial ,_mineGenerationVariables.MineGenView);
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
				var cellCrackMaterial =
					_mineCellCrackMaterial.CellCrackMaterials[0];//.FirstOrDefault(cellCrackMat =>
						// cellCrackMat.MaterialType == "Normal");
				MineSetCellConditions.SetTileMapCell(tempCellPos, _playerControllerVariables.MouseDirection, tempCell, cellCrackMaterial, _mineGenerationVariables.MineGenView);
			}
		}
	}
    
	#endregion
    
}