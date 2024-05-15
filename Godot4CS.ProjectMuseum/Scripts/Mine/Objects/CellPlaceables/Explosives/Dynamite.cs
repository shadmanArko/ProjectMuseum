using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.CellPlaceables.Explosives;

public partial class Dynamite : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    private MineCellCrackMaterial _mineCellCrackMaterial;

    [Export] private float _timer;
    [Export] private Label _timerLabel;
    public override void _Ready()
    {
        SubscribeToActions();
        InitializeDiInstaller();
        _timer = 6f;
    }

    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _mineCellCrackMaterial = ServiceRegistry.Resolve<MineCellCrackMaterial>();
    }

    #region Subscribe and UnSubscribe

    private void SubscribeToActions()
    {
        
    }

    private void UnSubscribeToActions()
    {
        
    }

    #endregion

    public override void _Process(double delta)
    {
        var isReadyToExplode = CheckForExplosionValidity((float)delta);
        _timer = Mathf.Clamp(_timer, 0, 1000);
        _timerLabel.Text = Mathf.CeilToInt(_timer).ToString();
        if (!isReadyToExplode) return;
        SetProcess(false);
        Explode();
    }

    private bool CheckForExplosionValidity(float delta)
    {
        if (!(_timer > 0)) return true;
        _timer -= delta;
        return false;
    }
    
    private void Explode()
    {
        var currentCellPos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
        GD.Print($"explosive position {currentCellPos.X},{currentCellPos.Y}");
        var cell = _mineGenerationVariables.GetCell(currentCellPos);
        var adjacentCells = new List<Vector2I> { Vector2I.Down, Vector2I.Left, Vector2I.Right, Vector2I.Up };
        var normalCellCrackMaterial =
            _mineCellCrackMaterial!.CellCrackMaterials.FirstOrDefault(cellCrackMat =>
                cellCrackMat.MaterialType == "Normal");
        
        foreach (var direction in adjacentCells)
        {
            var newCellPos = new Vector2I(cell.PositionX + direction.X, cell.PositionY + direction.Y);
            var adjacentCell = _mineGenerationVariables.GetCell(newCellPos);
            if(adjacentCell == null) continue;
            GD.Print($"Start explosion for {adjacentCell.PositionX},{adjacentCell.PositionY}");
            
            adjacentCell.HitPoint = adjacentCell.HasArtifact ? 1 : 0;
            MineSetCellConditions.SetCrackOnTiles(newCellPos, _playerControllerVariables.MouseDirection, adjacentCell,
                normalCellCrackMaterial);

            if (adjacentCell.HitPoint <= 0)
            {
                var cells = MineCellDestroyer.DestroyCellByPosition(newCellPos, _mineGenerationVariables);
                var caveCells = CaveControlManager.RevealCave(_mineGenerationVariables, cells);
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
                MineActions.OnMineCellBroken?.Invoke(newCellPos);
                GD.Print($"explosion affecting {adjacentCell.PositionX},{adjacentCell.PositionY}");
            }
            
            QueueFree();
        }
    }
    

    public override void _ExitTree()
    {
        UnSubscribeToActions();
        GD.Print("Dynamite exploded");
    }
}