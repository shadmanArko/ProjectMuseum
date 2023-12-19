using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;
using Godot4CS.ProjectMuseum.Scripts.Mine.ParticleEffects;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerCollisionWithWallDetector : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    private MineCellCrackMaterial _mineCellCrackMaterial;
    private List<RawArtifactDescriptive> _rawArtifactDescriptive;
    private List<RawArtifactFunctional> _rawArtifactFunctional;

    private HttpRequest _getMineArtifactHttpRequest;
    private HttpRequest _sendArtifactToInventoryHttpRequest;

    private RandomNumberGenerator _randomNumberGenerator;

    public override void _Ready()
    {
        CreateHttpRequests();
        InitializeDiReferences();
        SubscribeToActions();

        _randomNumberGenerator = new RandomNumberGenerator();
    }

    #region Initializers

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
        MineActions.OnDigActionEnded += AttackWall;
        MineActions.OnBrushActionStarted += BrushWall;

        MineActions.OnMiniGameWon += MiniGameWon;
        MineActions.OnMiniGameLost += MiniGameLost;
        MineActions.OnArtifactDiscoveryOkayButtonPressed += TurnOffArtifactDiscoveryScene;
    }

    #endregion

    #region Http Requests

    private void CreateHttpRequests()
    {
        _getMineArtifactHttpRequest = new HttpRequest();
        AddChild(_getMineArtifactHttpRequest);
        _getMineArtifactHttpRequest.RequestCompleted += OnGetMineArtifactHttpRequestCompleted;

        _sendArtifactToInventoryHttpRequest = new HttpRequest();
        AddChild(_sendArtifactToInventoryHttpRequest);
        _sendArtifactToInventoryHttpRequest.RequestCompleted += OnSendArtifactToInventoryHttpRequestCompleted;
    }

    #region Get Mine Artifact Request

    private void GetMineArtifact(string artifactId)
    {
        var url = ApiAddress.MineApiPath + "GetMineArtifactById/" + artifactId;
        _getMineArtifactHttpRequest.Request(url);
        GD.Print($"HTTP REQUEST FOR GETTING ARTIFACT BY ID FROM DATABASE (1)");
    }

    private void OnGetMineArtifactHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var artifact = JsonSerializer.Deserialize<Artifact>(jsonStr);
        var rawArtifact = _rawArtifactFunctional.FirstOrDefault(raw => raw.Id == artifact.RawArtifactId);

        var rand = new RandomNumberGenerator();
        var randomMat = rawArtifact!.Materials[rand.RandiRange(0, rawArtifact!.Materials.Count)];
        // var cellCrackMaterial =
        // 	_mineCellCrackMaterial.CellCrackMaterials.FirstOrDefault(mat => mat.MaterialType == randomMat);
        MineActions.OnArtifactSuccessfullyRetrieved?.Invoke(artifact);
        GD.Print($"HTTP REQUEST COMPLETED (2)");
    }

    #endregion

    #region Send Artifact To Inventory

    private void SendArtifactToInventory(string artifactId)
    {
        var url = $"{ApiAddress.MineApiPath}SendArtifactToInventory/{artifactId}";
        _sendArtifactToInventoryHttpRequest.Request(url);

        GD.Print($"HTTP REQUEST FOR SENDING ARTIFACT TO INVENTORY (1)");
    }

    private void OnSendArtifactToInventoryHttpRequestCompleted(long result, long responseCode, string[] headers,
        byte[] body)
    {
        GD.Print("Successfully sent artifact to inventory");
        var jsonStr = Encoding.UTF8.GetString(body);
        //var rawArtifactFunctionalList = JsonSerializer.Deserialize<List<RawArtifactFunctional>>(jsonStr);
        GD.Print("body " + jsonStr);
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
            _mineCellCrackMaterial.CellCrackMaterials.FirstOrDefault(tempCell =>
                tempCell.MaterialType == cell.ArtifactMaterial);

        MineSetCellConditions.SetArtifactCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection, cell,
            cellCrackMaterial, _mineGenerationVariables.MineGenView);

        _playerControllerVariables.CanMove = false;
        _artifactTilePos = tilePos;

        var scene =
            ResourceLoader.Load<PackedScene>(_alternateButtonPressMiniGameScenePath).Instantiate() as
                AlternateTapMiniGame;
        if (scene is null)
        {
            GD.PrintErr("COULD NOT instantiate Alternate tap mini game scene. FATAL ERROR");
            return;
        }

        AddChild(scene);
    }

    #region Mini Game

    private void MiniGameWon()
    {
        GD.Print("Successfully Extracted Artifact");

        ShowDiscoveredArtifact();
        var cell = _mineGenerationVariables.GetCell(_artifactTilePos);
        DigOrdinaryCell(_artifactTilePos);
        GD.Print("Sending artifact to inventory");
        GD.Print($"cell artifact id: {cell.ArtifactId}");
        SendArtifactToInventory(cell.ArtifactId);
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
        var scene =
            ResourceLoader.Load<PackedScene>(_discoveredArtifactScenePath)
                .Instantiate() as DiscoveredArtifactVisualizer;
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

    #endregion

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
        GD.Print($"artifact id: {cell.ArtifactId}");
        MineSetCellConditions.SetArtifactCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection, cell,
            cellCrackMaterial, _mineGenerationVariables.MineGenView);
        MakeMineWallDepletedParticleEffect();
        
        if (cell.HitPoint <= 0)
        {
            var cells = MineCellDestroyer.DestroyCellByPosition(tilePos, _mineGenerationVariables);

            GD.Print($"mouseDirection: {_playerControllerVariables.MouseDirection}");
            if (_playerControllerVariables.MouseDirection == Vector2I.Down)
            {
                GD.Print("is grounded is made false");
                if (_playerControllerVariables.State != MotionState.Hanging)
                    _playerControllerVariables.State = MotionState.Falling;
            }

            foreach (var tempCell in cells)
            {
                var tempCellPos = new Vector2I(tempCell.PositionX, tempCell.PositionY);
                MineSetCellConditions.SetTileMapCell(tempCellPos, _playerControllerVariables.MouseDirection, tempCell,
                    cellCrackMaterial, _mineGenerationVariables.MineGenView);
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
        MineSetCellConditions.SetCrackOnTiles(tilePos, _playerControllerVariables.MouseDirection, cell,
            normalCellCrackMaterial, _mineGenerationVariables.MineGenView);
        MakeMineWallDepletedParticleEffect();
        if (cell.HitPoint <= 0)
        {
            var cells = MineCellDestroyer.DestroyCellByPosition(tilePos, _mineGenerationVariables);

            GD.Print($"mouseDirection: {_playerControllerVariables.MouseDirection}");
            if (_playerControllerVariables.MouseDirection == Vector2I.Down)
            {
                GD.Print("is grounded is made false");
                if (_playerControllerVariables.State != MotionState.Hanging)
                    _playerControllerVariables.State = MotionState.Falling;
            }

            foreach (var tempCell in cells)
            {
                var tempCellPos = new Vector2I(tempCell.PositionX, tempCell.PositionY);
                var cellCrackMaterial =
                    _mineCellCrackMaterial.CellCrackMaterials[0]; //.FirstOrDefault(cellCrackMat =>
                // cellCrackMat.MaterialType == "Normal");
                MineSetCellConditions.SetTileMapCell(tempCellPos, _playerControllerVariables.MouseDirection, tempCell,
                    cellCrackMaterial, _mineGenerationVariables.MineGenView);
            }
        }
    }

    #region Wall Particle Effects

    private void MakeMineWallDepletedParticleEffect()
    {
        var particleEffectPath = ReferenceStorage.Instance.DepletedParticleExplosion;
        var particle = ResourceLoader.Load<PackedScene>(particleEffectPath).Instantiate() as DepletedParticleExplosion;
        if (particle == null) return;

        var position = _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
        position += _playerControllerVariables.MouseDirection;
        particle.Position = position * _mineGenerationVariables.Mine.CellSize;

        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var rand = _randomNumberGenerator.RandfRange(cellSize / 4f,cellSize);
        
        switch (_playerControllerVariables.MouseDirection)
        {
            case (1, 0):
                particle.Position += new Vector2(0, rand);
                particle.EmitParticle(_playerControllerVariables.MouseDirection);
                break;
            case (-1, 0):
                particle.Position += new Vector2(cellSize, rand);
                particle.EmitParticle(_playerControllerVariables.MouseDirection);
                break;
            case (0, -1):
                particle.Position += new Vector2(rand, cellSize);
                particle.EmitParticle(_playerControllerVariables.MouseDirection);
                break;
            case (0, 1):
                particle.Position += new Vector2(rand, 0);
                particle.EmitParticle(_playerControllerVariables.MouseDirection);
                break;
        }

        _mineGenerationVariables.MineGenView.AddChild(particle);
        GD.Print("Emitting particles");
        var direction = _playerControllerVariables.MouseDirection * -1;
        particle.EmitParticle(direction);
    }

    #endregion

    #endregion
}