using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerCollisionWithWallDetector : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    private MineCellCrackMaterial _mineCellCrackMaterial;

    private HttpRequest _getMineArtifactHttpRequest;
    private HttpRequest _mineCrackCellMaterialHttpRequest;

    

    public override void _Ready()
    {
        CreateHttpRequests();
        InitializeDiReferences();
        SubscribeToActions();
        GetMineCrackMaterialData();
    }

    #region Initializers

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _mineCellCrackMaterial = ServiceRegistry.Resolve<MineCellCrackMaterial>();
    }

    private void SubscribeToActions()
    {
        MineActions.OnPlayerCollisionDetection += DetectCollision;
        MineActions.OnDigActionEnded += AttackWall;
        MineActions.OnArtifactCellBroken += DigOrdinaryCell;
    }

    #endregion

    #region Http Requests

    private void CreateHttpRequests()
    {
        _getMineArtifactHttpRequest = new HttpRequest();
        AddChild(_getMineArtifactHttpRequest);
        _getMineArtifactHttpRequest.RequestCompleted += OnGetMineArtifactHttpRequestCompleted;

        _mineCrackCellMaterialHttpRequest = new HttpRequest();
        AddChild(_mineCrackCellMaterialHttpRequest);
        _mineCrackCellMaterialHttpRequest.RequestCompleted += OnGetMineCrackCellMaterialHttpRequestCompleted;
    }

    #region Get Mine Artifact Request

    private void GetMineArtifact(string artifactId)
    {
        var url = ApiAddress.MineApiPath + "GetMineArtifactById/" + artifactId;
        _getMineArtifactHttpRequest.Request(url);
    }

    private void OnGetMineArtifactHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var artifact = JsonSerializer.Deserialize<Artifact>(jsonStr);
        //MineActions.OnArtifactSuccessfullyRetrieved?.Invoke(artifact);
    }

    #endregion

    #region Get Mine Cell Crack Materials

    private void GetMineCrackMaterialData()
    {
        var url = ApiAddress.MineApiPath+"GetAllMineCellCrackMaterials";
        _mineCrackCellMaterialHttpRequest.Request(url);
    }
    
    private void OnGetMineCrackCellMaterialHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var cellCrackMaterials = JsonSerializer.Deserialize<List<CellCrackMaterial>>(jsonStr);
        _mineCellCrackMaterial.CellCrackMaterials = cellCrackMaterials;
    }

    #endregion

    #endregion

    private void DetectCollision(KinematicCollision2D collision)
    {
        var tileMap = _mineGenerationVariables.MineGenView.TileMap;

        if (collision.GetCollider() == tileMap)
        {
            var state = _playerControllerVariables.State;
            var tilePos = _mineGenerationVariables.MineGenView.TileMap.LocalToMap(_playerControllerVariables.Position);
            tilePos += Vector2I.Down;
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
        if (cell.HasArtifact)
            DigArtifactCell(targetTilePosition);
        else
            DigOrdinaryCell(targetTilePosition);

        MineActions.OnSuccessfulDigActionCompleted?.Invoke();
    }

    [Export] private string _alternateButtonPressMiniGameScenePath;
    [Export] private bool _isMiniGameLoaded;
    private Vector2I _artifactTilePos;
    
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
        var cell = _mineGenerationVariables.GetCell(tilePos);
        
        cell.HitPoint -= _playerControllerVariables.CellDamagePoint;
        Math.Clamp(-_mineGenerationVariables.GetCell(tilePos).HitPoint, 0, 100000);
        
        var cellCrackMaterial =
            _mineCellCrackMaterial!.CellCrackMaterials.FirstOrDefault(cellCrackMat =>
                cellCrackMat.MaterialType == "Normal");
        MineSetCellConditions.SetCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection, cell,
            cellCrackMaterial);
        
        if (cell.HitPoint <= 0)
        {
            MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("OnDigFirstArtifactCell");
            MineActions.OnMiniGameLoad?.Invoke(tilePos);
            var cells = MineCellDestroyer.DestroyCellByPosition(tilePos, _mineGenerationVariables);
            
            if (_playerControllerVariables.MouseDirection == Vector2I.Down)
            {
                if (_playerControllerVariables.State != MotionState.Hanging)
                    _playerControllerVariables.State = MotionState.Falling;
            }

            foreach (var tempCell in cells)
            {
                var tempCellPos = new Vector2I(tempCell.PositionX, tempCell.PositionY);
                MineSetCellConditions.SetTileMapCell(tempCellPos, _playerControllerVariables.MouseDirection, tempCell,
                    cellCrackMaterial, _mineGenerationVariables);
            }
        }
    }

    private void DigOrdinaryCell(Vector2I tilePos)
    {
        var cell = _mineGenerationVariables.GetCell(tilePos);
        cell.HitPoint -= _playerControllerVariables.CellDamagePoint;
        Math.Clamp(-_mineGenerationVariables.GetCell(tilePos).HitPoint, 0, 100000);
        // GD.PrintErr($"cell HP: {cell.HitPoint}");
        var normalCellCrackMaterial =
            _mineCellCrackMaterial!.CellCrackMaterials.FirstOrDefault(cellCrackMat =>
                cellCrackMat.MaterialType == "Normal");
        MineSetCellConditions.SetCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection, cell,
            normalCellCrackMaterial);
        if (cell.HitPoint <= 0)
        {
            var cells = MineCellDestroyer.DestroyCellByPosition(tilePos, _mineGenerationVariables);
            var caveCells = CaveControlManager.RevealCave(_mineGenerationVariables, cells);
            
            if (_playerControllerVariables.MouseDirection == Vector2I.Down)
            {
                if (_playerControllerVariables.State != MotionState.Hanging)
                    _playerControllerVariables.State = MotionState.Falling;
            }

            foreach (var tempCell in cells)
            {
                var tempCellPos = new Vector2I(tempCell.PositionX, tempCell.PositionY);
                var cellCrackMaterial =
                    _mineCellCrackMaterial.CellCrackMaterials[0];
                MineSetCellConditions.SetTileMapCell(tempCellPos, _playerControllerVariables.MouseDirection, tempCell,
                    cellCrackMaterial, _mineGenerationVariables);
            }
            
            foreach (var tempCell in caveCells)
            {
                var tempCellPos = new Vector2I(tempCell.PositionX, tempCell.PositionY);
                var cellCrackMaterial =
                    _mineCellCrackMaterial.CellCrackMaterials[0];
                MineSetCellConditions.SetTileMapCell(tempCellPos, _playerControllerVariables.MouseDirection, tempCell,
                    cellCrackMaterial, _mineGenerationVariables);
            }

            _mineGenerationVariables.BrokenCells++;
            MineActions.OnMineCellBroken?.Invoke(tilePos);
            MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("OnDigFirstOrdinaryCell");
        }
    }

    #endregion

    private void OnCellBlockEnter(Node2D body)
    {
        var hasCollidedWithMine = body == _mineGenerationVariables.MineGenView;
        if (hasCollidedWithMine)
        {
            _playerControllerVariables.State = MotionState.Grounded;
            _playerControllerVariables.Acceleration = _playerControllerVariables.State == MotionState.Hanging ? 
                PlayerControllerVariables.MaxSpeed / 2 : PlayerControllerVariables.MaxSpeed;
        }
    }

    private void OnCellBlockExit(Node2D body)
    {
        var hasCollidedWithMine = body == _mineGenerationVariables.MineGenView;
        if (hasCollidedWithMine && _playerControllerVariables.State != MotionState.Hanging)
        {
            _playerControllerVariables.State = MotionState.Falling;
            // GD.Print($"player state = {_playerControllerVariables.State}");
        }
    }
}